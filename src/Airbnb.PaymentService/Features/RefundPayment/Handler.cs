using Airbnb.PaymentService.Domain;
using Airbnb.PaymentService.Infrastructure;
using Airbnb.PaymentService.Infrastructure.HttpClients;
using Airbnb.ServiceDefaults.Infrastructure;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.PaymentService.Features.RefundPayment;

public sealed class Handler(
    PaymentDbContext db,
    ILogger<Handler> logger) : ICommandHandler<Command, Result>
{
    public async ValueTask<Result> Handle(Command req, CancellationToken ct)
    {
        // 1. Load payment + history of prior refunds.
        var payment = await db.Payments.FirstOrDefaultAsync(p => p.Id == req.PaymentId, ct)
            ?? throw new BusinessException("Payment not found.", "REFUND_PAYMENT_NOT_FOUND");

        if (payment.Status != PaymentStatus.Success && payment.Status != PaymentStatus.PartiallyRefunded)
            throw new BusinessException(
                $"Cannot refund a payment in status {payment.Status}.",
                "REFUND_INVALID_STATUS");

        var priorRefunded = await db.RefundRecords
            .Where(r => r.PaymentId == payment.Id)
            .SumAsync(r => (decimal?)r.Amount, ct) ?? 0m;

        var remaining = payment.Amount - priorRefunded;
        if (req.Amount > remaining)
            throw new BusinessException(
                $"Refund amount {req.Amount} exceeds remaining {remaining}.",
                "REFUND_AMOUNT_EXCEEDS_REMAINING");

        var isFullRefund = (priorRefunded + req.Amount) == payment.Amount;

        // 2. Reject if payment has already been paid out to host (cannot claw back from bank).
        var paidOut = await db.BalanceEntries
            .AnyAsync(e => e.PaymentId == payment.Id && e.Type == BalanceEntryType.PayoutApproved, ct);
        if (paidOut)
            throw new BusinessException(
                "Payment is already paid out to host; refunds must be handled out-of-band.",
                "REFUND_ALREADY_PAID_OUT");

        // Find the original PaymentReceived entry to know the host portion split.
        var receivedEntry = await db.BalanceEntries
            .FirstOrDefaultAsync(e => e.PaymentId == payment.Id && e.Type == BalanceEntryType.PaymentReceived, ct);

        // Determine bucket: if BookingCheckedOut already happened, money is in Available; else Pending.
        var hasCheckedOut = await db.BalanceEntries
            .AnyAsync(e => e.PaymentId == payment.Id && e.Type == BalanceEntryType.BookingCheckedOut, ct);

        // 3. Insert RefundRecord + ledger entry + update HostBalance (atomic).
        var refund = RefundRecord.Create(
            payment.Id, req.Amount, req.Reason, isFullRefund, req.PerformedBy, req.TicketId);
        db.RefundRecords.Add(refund);

        if (receivedEntry is not null)
        {
            var hostId = receivedEntry.HostId;

            // Refund proportionally on host portion: (refundAmount / paymentAmount) * pendingDelta (host portion).
            var hostPortionTotal = receivedEntry.PendingDelta; // = host's share of the full payment
            var hostPortionRefund = Math.Round(hostPortionTotal * req.Amount / payment.Amount, 2);

            var balance = await db.HostBalances
                .FirstOrDefaultAsync(b => b.HostId == hostId && b.Currency == payment.Currency, ct);

            if (balance is not null)
            {
                var ledgerEntry = BalanceEntry.Refund(
                    hostId, hostPortionRefund, payment.Currency, payment.Id, payment.BookingId,
                    fromPending: !hasCheckedOut);
                db.BalanceEntries.Add(ledgerEntry);
                balance.ApplyEntry(ledgerEntry);
            }
            else
            {
                logger.LogWarning(
                    "No HostBalance for host {HostId}/{Currency} during refund of Payment {PaymentId}. " +
                    "Skipping ledger update.", hostId, payment.Currency, payment.Id);
            }
        }
        else
        {
            logger.LogWarning(
                "No PaymentReceived ledger entry for Payment {PaymentId}; ledger refund skipped.",
                payment.Id);
        }

        // 4. Transition payment status (raises PaymentRefundedDomainEvent → outbox).
        if (isFullRefund)
            payment.MarkAsRefunded(req.Amount, req.Reason);
        else
            payment.MarkAsPartiallyRefunded(req.Amount, req.Reason);

        await db.SaveChangesAsync(ct);

        logger.LogInformation(
            "Refunded {Amount} {Currency} on Payment {PaymentId} (full={Full}). Total now {Total}/{PaymentAmount}.",
            req.Amount, payment.Currency, payment.Id, isFullRefund,
            priorRefunded + req.Amount, payment.Amount);

        return new Result(
            refund.Id,
            payment.Id,
            req.Amount,
            priorRefunded + req.Amount,
            isFullRefund);
    }
}

using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.PaymentService.Domain;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Features.Admin.ApprovePayout;

public sealed class ApprovePayoutHandler(PaymentDbContext db, ILogger<ApprovePayoutHandler> logger)
    : ICommandHandler<Request, Response>
{
    public async ValueTask<Response> Handle(Request req, CancellationToken ct)
    {
        var payout = await db.Payouts
            .Include(p => p.Items)
            .FirstOrDefaultAsync(p => p.Id == req.Id, ct)
            ?? throw new NotFoundException("Payout not found.");

        payout.Approve(req.PerformedBy);

        var balance = await db.HostBalances
            .FirstOrDefaultAsync(b => b.HostId == payout.HostId && b.Currency == payout.Currency, ct)
            ?? throw new BusinessException(
                $"No balance ledger found for host {payout.HostId} / {payout.Currency}. " +
                "PaymentSucceededEvent consumer may not have run.",
                "PAYOUT_NO_LEDGER");

        var paymentIds = payout.Items.Select(i => i.PaymentId).ToList();

        var alreadyReleased = await db.BalanceEntries
            .Where(e => e.Type == BalanceEntryType.BookingCheckedOut
                     && paymentIds.Contains(e.PaymentId!.Value))
            .Select(e => e.PaymentId!.Value)
            .ToListAsync(ct);

        var receivedEntries = await db.BalanceEntries
            .Where(e => e.Type == BalanceEntryType.PaymentReceived
                     && paymentIds.Contains(e.PaymentId!.Value))
            .ToListAsync(ct);

        foreach (var item in payout.Items)
        {
            if (alreadyReleased.Contains(item.PaymentId)) continue;

            var received = receivedEntries.FirstOrDefault(e => e.PaymentId == item.PaymentId);
            if (received is null)
            {
                logger.LogWarning("No PaymentReceived entry for Payment {PaymentId} in payout {PayoutId}; skipping release.",
                    item.PaymentId, payout.Id);
                continue;
            }

            var releaseEntry = BalanceEntry.BookingCheckedOut(
                payout.HostId, received.PendingDelta, payout.Currency, item.PaymentId, item.BookingId);
            db.BalanceEntries.Add(releaseEntry);
            balance.ApplyEntry(releaseEntry);
        }

        var alreadyDeducted = await db.BalanceEntries
            .AnyAsync(e => e.PayoutId == payout.Id && e.Type == BalanceEntryType.PayoutApproved, ct);
        if (!alreadyDeducted)
        {
            var deductEntry = BalanceEntry.PayoutApproved(
                payout.HostId, payout.PayoutAmount, payout.Currency, payout.Id);
            db.BalanceEntries.Add(deductEntry);
            balance.ApplyEntry(deductEntry);
        }

        await db.SaveChangesAsync(ct);

        return new Response(payout.Id, payout.Status, payout.ApprovedAt);
    }
}

using Airbnb.PaymentService.Domain;
using Airbnb.PaymentService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.PaymentService.Features.RefundPayment;

public sealed class Handler(PaymentDbContext db, ILogger<Handler> logger)
    : ICommandHandler<Command, RefundPaymentResponse>
{
    public async ValueTask<RefundPaymentResponse> Handle(Command req, CancellationToken ct)
    {
        var payment = await db.Payments.FirstOrDefaultAsync(p => p.Id == req.PaymentId, ct);

        if (payment is null)
            throw new BusinessException("Payment not found", "PAYMENT_NOT_FOUND");

        if (payment.Status != PaymentStatus.Success && payment.Status != PaymentStatus.PartiallyRefunded)
            throw new BusinessException("Only successful or partially refunded payments can be refunded", "PAYMENT_REFUND_INVALID_STATUS");

        // Calculate remaining refundable amount
        var totalRefunded = await db.RefundRecords
            .Where(r => r.PaymentId == req.PaymentId)
            .SumAsync(r => r.Amount, ct);

        var remainingRefundable = payment.Amount - totalRefunded;

        var isFullRefund = !req.Amount.HasValue || req.Amount.Value >= remainingRefundable;
        var refundAmount = isFullRefund ? remainingRefundable : req.Amount!.Value;

        if (refundAmount <= 0)
            throw new BusinessException("No remaining refundable amount", "PAYMENT_FULLY_REFUNDED");

        if (refundAmount > remainingRefundable)
            throw new BusinessException($"Refund amount ({refundAmount}) exceeds remaining refundable amount ({remainingRefundable})", "PAYMENT_REFUND_AMOUNT_EXCEEDED");

        var refund = RefundRecord.Create(
            req.PaymentId,
            refundAmount,
            req.Reason,
            isFullRefund,
            Guid.Empty, // TODO: Get admin user ID from claims
            req.TicketId
        );

        db.RefundRecords.Add(refund);

        // Update payment status
        if (isFullRefund)
            payment.MarkAsRefunded();
        else
            payment.MarkAsPartiallyRefunded();

        await db.SaveChangesAsync(ct);

        logger.LogInformation("Refund {RefundId} created for Payment {PaymentId}, amount: {Amount}, full: {IsFull}",
            refund.Id, req.PaymentId, refundAmount, isFullRefund);

        return new RefundPaymentResponse(
            refund.Id,
            req.PaymentId,
            refundAmount,
            isFullRefund,
            (int)payment.Status
        );
    }
}

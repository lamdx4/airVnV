using Airbnb.PaymentService.Domain;
using Airbnb.PaymentService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;
using Airbnb.SharedKernel.Events;
using MassTransit;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.PaymentService.Features.Consumers;

/// <summary>
/// Triggered by BookingStateMachine (Orchestration) when a Confirmed booking is being cancelled.
/// Attempts a full refund and publishes outcome events so the Saga can update its state.
/// </summary>
public class RefundPaymentCommandConsumer(
    PaymentDbContext db,
    IMediator mediator,
    ILogger<RefundPaymentCommandConsumer> logger)
    : IConsumer<RefundPaymentCommand>
{
    public async Task Consume(ConsumeContext<RefundPaymentCommand> ctx)
    {
        var msg = ctx.Message;

        var payment = await db.Payments
            .Where(p => p.BookingId == msg.BookingId
                     && (p.Status == PaymentStatus.Success || p.Status == PaymentStatus.PartiallyRefunded))
            .OrderByDescending(p => p.CreatedAt)
            .FirstOrDefaultAsync(ctx.CancellationToken);

        if (payment is null)
        {
            logger.LogInformation(
                "No refundable payment for Booking {BookingId} (RefundPaymentCommand). Reason: {Reason}. Publishing RefundPaymentFailedEvent.",
                msg.BookingId, msg.Reason);

            // No payment exists to refund — this is a non-retryable terminal failure.
            await ctx.Publish(new RefundPaymentFailedEvent(
                msg.BookingId,
                "REFUND_PAYMENT_NOT_FOUND",
                "No refundable payment found for this booking."));
            return;
        }

        var priorRefunded = await db.RefundRecords
            .Where(r => r.PaymentId == payment.Id)
            .SumAsync(r => (decimal?)r.Amount, ctx.CancellationToken) ?? 0m;
        var remaining = payment.Amount - priorRefunded;
        if (remaining <= 0)
        {
            logger.LogInformation("Payment {PaymentId} already fully refunded. Booking {BookingId}.", payment.Id, msg.BookingId);
            return;
        }

        var systemId = Guid.Parse("00000000-0000-0000-0000-000000000099");

        try
        {
            await mediator.Send(
                new Features.RefundPayment.Command(payment.Id, remaining, msg.Reason, systemId),
                ctx.CancellationToken);

            logger.LogInformation(
                "Issued auto-refund of {Amount} on Payment {PaymentId} for Booking {BookingId}. Reason: {Reason}",
                remaining, payment.Id, msg.BookingId, msg.Reason);
        }
        catch (BusinessException ex)
        {
            // BusinessException (e.g. REFUND_ALREADY_PAID_OUT) is non-retryable.
            // Publish RefundPaymentFailedEvent so the Saga can transition to RefundFailed
            // instead of being stuck in the Refunding state indefinitely.
            logger.LogCritical(ex,
                "Non-retryable refund failure for Booking {BookingId}, Payment {PaymentId}. ErrorCode: {ErrorCode}. Publishing RefundPaymentFailedEvent.",
                msg.BookingId, payment.Id, ex.Message);

            await ctx.Publish(new RefundPaymentFailedEvent(
                msg.BookingId,
                ex.Message,
                $"Refund failed permanently: {ex.Message}"));
        }
    }
}

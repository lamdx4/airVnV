using Airbnb.PaymentService.Domain;
using Airbnb.PaymentService.Infrastructure;
using Airbnb.SharedKernel.Events;
using MassTransit;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.PaymentService.Features.Consumers;

/// <summary>
/// Triggered by BookingStateMachine when host approval times out (24h) or
/// admin cancels a confirmed booking. Issues a full refund on the (single) Success
/// payment tied to that booking.
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

        // Find the Success/PartiallyRefunded payment for this booking.
        var payment = await db.Payments
            .Where(p => p.BookingId == msg.BookingId
                     && (p.Status == PaymentStatus.Success || p.Status == PaymentStatus.PartiallyRefunded))
            .OrderByDescending(p => p.CreatedAt)
            .FirstOrDefaultAsync(ctx.CancellationToken);

        if (payment is null)
        {
            logger.LogInformation(
                "No refundable payment for Booking {BookingId} (RefundPaymentCommand). Reason: {Reason}",
                msg.BookingId, msg.Reason);
            return;
        }

        var priorRefunded = await db.RefundRecords
            .Where(r => r.PaymentId == payment.Id)
            .SumAsync(r => (decimal?)r.Amount, ctx.CancellationToken) ?? 0m;
        var remaining = payment.Amount - priorRefunded;
        if (remaining <= 0)
        {
            logger.LogInformation("Payment {PaymentId} already fully refunded.", payment.Id);
            return;
        }

        // Use a synthetic "system" performer for saga-triggered refunds.
        var systemId = Guid.Parse("00000000-0000-0000-0000-000000000099");

        await mediator.Send(
            new Features.RefundPayment.Command(payment.Id, remaining, msg.Reason, systemId),
            ctx.CancellationToken);
    }
}

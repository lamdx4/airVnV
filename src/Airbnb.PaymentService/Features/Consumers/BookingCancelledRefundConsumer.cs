using Airbnb.PaymentService.Domain;
using Airbnb.PaymentService.Infrastructure;
using Airbnb.SharedKernel.Events;
using MassTransit;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.PaymentService.Features.Consumers;

/// <summary>
/// When a booking gets cancelled (guest/host cancel after payment succeeded, or
/// admin cancel), refund the underlying Success payment and reverse the host
/// ledger. Runs in parallel to the saga so refunds work even if the saga is
/// offline for a given booking.
/// </summary>
public class BookingCancelledRefundConsumer(
    PaymentDbContext db,
    IMediator mediator,
    ILogger<BookingCancelledRefundConsumer> logger)
    : IConsumer<BookingCancelledEvent>
{
    public async Task Consume(ConsumeContext<BookingCancelledEvent> ctx)
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
                "BookingCancelled for {BookingId}: no refundable payment found.", msg.BookingId);
            return;
        }

        var priorRefunded = await db.RefundRecords
            .Where(r => r.PaymentId == payment.Id)
            .SumAsync(r => (decimal?)r.Amount, ctx.CancellationToken) ?? 0m;
        var remaining = payment.Amount - priorRefunded;
        if (remaining <= 0)
        {
            logger.LogInformation(
                "BookingCancelled for {BookingId}: Payment {PaymentId} already fully refunded.",
                msg.BookingId, payment.Id);
            return;
        }

        var systemId = Guid.Parse("00000000-0000-0000-0000-000000000099");

        await mediator.Send(
            new Features.RefundPayment.Command(payment.Id, remaining, msg.Reason, systemId),
            ctx.CancellationToken);

        logger.LogInformation(
            "Issued auto-refund of {Amount} on Payment {PaymentId} for cancelled Booking {BookingId}. Reason: {Reason}",
            remaining, payment.Id, msg.BookingId, msg.Reason);
    }
}

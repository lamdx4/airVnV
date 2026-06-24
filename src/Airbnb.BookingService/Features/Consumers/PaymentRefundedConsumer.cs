using Airbnb.SharedKernel.Events;
using MassTransit;
using Airbnb.BookingService.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Airbnb.BookingService.Domain;

namespace Airbnb.BookingService.Features.Consumers;

/// <summary>
/// When a payment is refunded (full), cancel the linked booking if still active.
/// Partial refunds keep the booking active.
/// </summary>
public class PaymentRefundedConsumer(
    ILogger<PaymentRefundedConsumer> logger,
    BookingDbContext dbContext) : IConsumer<PaymentRefundedEvent>
{
    public async Task Consume(ConsumeContext<PaymentRefundedEvent> context)
    {
        var message = context.Message;
        if (!message.IsFullRefund)
        {
            logger.LogInformation(
                "Partial refund for Booking {BookingId}, no booking state change.",
                message.BookingId);
            return;
        }

        var booking = await dbContext.Bookings.FindAsync([message.BookingId], context.CancellationToken);
        if (booking is null)
        {
            logger.LogWarning("Booking {BookingId} not found for refund.", message.BookingId);
            return;
        }

        if (booking.Status == BookingStatus.Refunding)
        {
            // Happy path: Saga orchestrated the refund, now complete the cancellation.
            booking.CompleteRefundCancellation();
        }
        else if (booking.Status == BookingStatus.Cancelled)
        {
            logger.LogInformation("Booking {BookingId} already cancelled.", message.BookingId);
        }
        else
        {
            // Fallback for any other states (e.g., admin-triggered refunds out-of-band).
            booking.AdminCancel();
        }

        await dbContext.SaveChangesAsync(context.CancellationToken);

        logger.LogInformation(
            "Booking {BookingId} cancelled due to full refund ({Amount} {Currency}).",
            message.BookingId, message.RefundAmount, message.Currency);
    }
}

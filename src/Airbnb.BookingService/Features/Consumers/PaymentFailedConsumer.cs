using Airbnb.SharedKernel.Events;
using MassTransit;
using Airbnb.BookingService.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Airbnb.BookingService.Domain;

namespace Airbnb.BookingService.Features.Consumers;

public class PaymentFailedConsumer(
    ILogger<PaymentFailedConsumer> logger,
    BookingDbContext dbContext) : IConsumer<PaymentFailedEvent>
{
    public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
    {
        var message = context.Message;
        logger.LogInformation("Processing PaymentFailedEvent for Booking {BookingId}", message.BookingId);

        var booking = await dbContext.Bookings.FindAsync([message.BookingId], context.CancellationToken);
        if (booking == null)
        {
            logger.LogWarning("Booking {BookingId} not found.", message.BookingId);
            return;
        }

        booking.Cancel(booking.GuestId);

        await dbContext.SaveChangesAsync(context.CancellationToken);
        
        logger.LogInformation("Booking {BookingId} cancelled successfully due to payment failure.", message.BookingId);
    }
}

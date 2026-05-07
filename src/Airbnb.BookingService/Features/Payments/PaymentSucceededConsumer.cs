using Airbnb.BookingService.Domain;
using Airbnb.BookingService.Infrastructure;
using Airbnb.SharedKernel.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.BookingService.Features.Payments;

public class PaymentSucceededConsumer(
    BookingDbContext db,
    ILogger<PaymentSucceededConsumer> logger)
    : IConsumer<PaymentSucceededEvent>
{
    public async Task Consume(ConsumeContext<PaymentSucceededEvent> context)
    {
        var bookingId = context.Message.BookingId;
        logger.LogInformation("Processing PaymentSucceededEvent for Booking {BookingId}", bookingId);

        var booking = await db.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId);
        if (booking == null)
        {
            logger.LogWarning("Booking {BookingId} not found for PaymentSucceededEvent", bookingId);
            return;
        }

        if (booking.Status != BookingStatus.Pending)
        {
            logger.LogInformation("Booking {BookingId} already in status {Status}, skipping confirmation", bookingId, booking.Status);
            return;
        }

        booking.Confirm();
        await db.SaveChangesAsync();

        logger.LogInformation("Booking {BookingId} confirmed successfully", bookingId);
    }
}

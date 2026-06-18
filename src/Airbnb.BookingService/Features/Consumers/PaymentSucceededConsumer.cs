using Airbnb.SharedKernel.Events;
using MassTransit;
using Airbnb.BookingService.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Airbnb.BookingService.Domain;

namespace Airbnb.BookingService.Features.Consumers;

public class PaymentSucceededConsumer(
    ILogger<PaymentSucceededConsumer> logger,
    BookingDbContext dbContext) : IConsumer<PaymentSucceededEvent>
{
    public async Task Consume(ConsumeContext<PaymentSucceededEvent> context)
    {
        var message = context.Message;
        logger.LogInformation("Processing PaymentSucceededEvent for Booking {BookingId}", message.BookingId);

        var booking = await dbContext.Bookings.FindAsync([message.BookingId], context.CancellationToken);
        if (booking == null)
        {
            logger.LogWarning("Booking {BookingId} not found.", message.BookingId);
            return;
        }

        if (booking.BookingMode == Airbnb.BookingService.Domain.Enums.BookingMode.InstantBook)
        {
            booking.Confirm();
        }
        else
        {
            booking.AwaitForApproval();
        }

        await dbContext.SaveChangesAsync(context.CancellationToken);
        
        logger.LogInformation("Booking {BookingId} updated successfully based on payment success.", message.BookingId);
    }
}

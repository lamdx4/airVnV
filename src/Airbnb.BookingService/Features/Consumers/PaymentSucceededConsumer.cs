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
        var eventId = context.MessageId ?? Guid.NewGuid();

        logger.LogInformation("Processing PaymentSucceededEvent for Booking {BookingId}", message.BookingId);

        // Idempotency check
        bool alreadyProcessed = await dbContext.ProcessedEvents.AnyAsync(e => e.EventId == eventId, context.CancellationToken);
        if (alreadyProcessed)
        {
            logger.LogWarning("Event {EventId} already processed. Skipping.", eventId);
            return;
        }

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

        // Đánh dấu đã xử lý
        dbContext.ProcessedEvents.Add(new ProcessedEvent 
        { 
            EventId = eventId, 
            EventType = nameof(PaymentSucceededEvent)
        });

        await dbContext.SaveChangesAsync(context.CancellationToken);
        
        logger.LogInformation("Booking {BookingId} updated successfully based on payment success.", message.BookingId);
    }
}

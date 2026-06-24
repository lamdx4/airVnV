using Airbnb.BookingService.Domain;
using Airbnb.BookingService.Infrastructure;
using Airbnb.SharedKernel.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Airbnb.BookingService.Features.Consumers;

public class BookingCancelledConsumer(
    BookingDbContext db,
    ILogger<BookingCancelledConsumer> logger) : IConsumer<BookingCancelledEvent>
{
    public async Task Consume(ConsumeContext<BookingCancelledEvent> context)
    {
        var bookingId = context.Message.BookingId;
        
        var booking = await db.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId, context.CancellationToken);
        if (booking == null)
        {
            logger.LogWarning("BookingCancelledEvent received for non-existent Booking {BookingId}", bookingId);
            return;
        }

        // Idempotency: already cancelled → skip silently to prevent infinite loop.
        // IMPORTANT: Do NOT call booking.AdminCancel() here — it raises BookingCancelledDomainEvent
        // which would be dispatched via Outbox → RabbitMQ → this Consumer again → infinite loop!
        if (booking.Status == BookingStatus.Cancelled)
        {
            logger.LogInformation("Booking {BookingId} is already Cancelled. Skipping.", bookingId);
            return;
        }

        // Directly sync the domain status without raising a new domain event.
        // Using SyncCancelled() instead of AdminCancel() to avoid:
        // AdminCancel() → raises BookingCancelledDomainEvent → Outbox → BookingCancelledEvent → this Consumer again → ∞ loop!
        booking.SyncCancelled(context.Message.Reason ?? "Cancelled by system");

        await db.SaveChangesAsync(context.CancellationToken);

        logger.LogInformation("Booking {BookingId} synced to Cancelled in Domain.", bookingId);
    }
}


using Airbnb.BookingService.Infrastructure;
using Airbnb.BookingService.Infrastructure.Saga;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Airbnb.BookingService.Features.Consumers;

public class BookingApprovalTimeoutConsumer(
    BookingDbContext db,
    ILogger<BookingApprovalTimeoutConsumer> logger) : IConsumer<BookingApprovalTimeoutEvent>
{
    public async Task Consume(ConsumeContext<BookingApprovalTimeoutEvent> context)
    {
        var bookingId = context.Message.BookingId;
        
        var booking = await db.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId, context.CancellationToken);
        if (booking == null)
        {
            logger.LogWarning("Timeout Event received for non-existent Booking {BookingId}", bookingId);
            return;
        }

        try
        {
            // We use System Guid to represent System cancellation
            var systemId = Guid.Empty; 
            booking.Cancel(systemId);
            await db.SaveChangesAsync(context.CancellationToken);
            
            logger.LogInformation("Booking {BookingId} cancelled by System due to Host Approval Timeout.", bookingId);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Failed to cancel Booking {BookingId} during timeout. Current Status: {Status}", bookingId, booking.Status);
        }
    }
}

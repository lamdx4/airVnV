using Airbnb.BookingService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.BookingService.Features.ApproveBooking;

public sealed class Handler(BookingDbContext db) : ICommandHandler<Request>
{
    public async ValueTask<Unit> Handle(Request req, CancellationToken ct)
    {
        var booking = await db.Bookings.FirstOrDefaultAsync(b => b.Id == req.BookingId, ct);
        if (booking == null)
            throw new NotFoundException("Booking not found.");

        // Domain method will check if HostId matches and if Status is Pending
        try
        {
            booking.Approve(req.HostId);
        }
        catch (InvalidOperationException ex)
        {
            throw new BusinessException(ex.Message, "BOOKING_APPROVE_ERROR");
        }

        await db.SaveChangesAsync(ct);
        return Unit.Value;
    }
}

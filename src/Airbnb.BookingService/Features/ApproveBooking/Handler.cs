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

        // Domain method checks HostId ownership; Approve() now calls AwaitForApproval()
        // which raises BookingAwaitingApprovalDomainEvent → Saga triggers payment flow
        booking.Approve(req.HostId);

        await db.SaveChangesAsync(ct);
        return Unit.Value;
    }
}

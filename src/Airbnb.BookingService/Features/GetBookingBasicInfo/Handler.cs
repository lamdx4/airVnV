using Airbnb.BookingService.Infrastructure;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.BookingService.Features.GetBookingBasicInfo;

public sealed class Handler(BookingDbContext db)
    : IQueryHandler<Request, Response>
{
    public async ValueTask<Response> Handle(Request req, CancellationToken ct)
    {
        var booking = await db.Bookings
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == req.Id, ct);

        if (booking == null)
            throw new Airbnb.ServiceDefaults.Infrastructure.NotFoundException("Booking not found");

        return new Response(
            booking.Id,
            booking.TotalPrice,
            booking.CurrencyCode,
            booking.CountryCode,
            booking.GuestId,
            booking.HostId);
    }
}

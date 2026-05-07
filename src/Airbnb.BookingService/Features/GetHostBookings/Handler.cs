using Airbnb.BookingService.Infrastructure;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.BookingService.Features.GetHostBookings;

public sealed class Handler(BookingDbContext db) : IQueryHandler<Request, List<BookingDto>>
{
    public async ValueTask<List<BookingDto>> Handle(Request req, CancellationToken ct)
    {
        return await db.Bookings
            .AsNoTracking()
            .Where(b => b.HostId == req.HostId)
            .OrderByDescending(b => b.CreatedAt)
            .Select(b => new BookingDto(
                b.Id,
                b.PropertyId,
                b.GuestId,
                b.CheckIn,
                b.CheckOut,
                b.GuestCount,
                b.NightCount,
                b.TotalPrice,
                b.CurrencyCode,
                b.Status.ToString()))
            .ToListAsync(ct);
    }
}

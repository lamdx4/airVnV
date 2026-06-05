using Microsoft.EntityFrameworkCore;
using Airbnb.BookingService.Infrastructure;
using Airbnb.BookingService.Domain;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.BookingService.Features.GetTopPropertiesAdmin;

public sealed class Handler(BookingDbContext db)
    : Mediator.IQueryHandler<Request, ApiResponse<List<TopPropertyBasic>>>
{
    public async ValueTask<ApiResponse<List<TopPropertyBasic>>> Handle(Request req, CancellationToken ct)
    {
        var limit = Math.Clamp(req.Limit, 1, 50);

        // Use the overlap with [from, to] to determine if a booking is in-range.
        // A booking "contributes" to revenue if it was created in the range AND is Confirmed.
        // Occupancy for a property = (nights of its Confirmed bookings overlapping the range) / rangeDays.
        var fromStart = req.From.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var toEnd = req.To.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);
        var rangeDays = Math.Max(1, req.To.DayNumber - req.From.DayNumber + 1);

        var confirmed = await db.Bookings.AsNoTracking()
            .Where(b => b.Status == BookingStatus.Confirmed)
            .Where(b => b.CreatedAt >= fromStart && b.CreatedAt <= toEnd)
            .Select(b => new
            {
                b.PropertyId,
                b.TotalPrice,
                Nights = b.CheckOut.DayNumber - b.CheckIn.DayNumber
            })
            .ToListAsync(ct);

        var result = confirmed
            .GroupBy(x => x.PropertyId)
            .Select(g => new TopPropertyBasic(
                PropertyId: g.Key,
                Revenue: g.Sum(x => x.TotalPrice),
                Bookings: g.Count(),
                OccupancyRate: Math.Round(Math.Min(1m, (decimal)g.Sum(x => x.Nights) / rangeDays), 4)
            ))
            .OrderByDescending(p => p.Revenue)
            .ThenByDescending(p => p.Bookings)
            .Take(limit)
            .ToList();

        return ApiResponse<List<TopPropertyBasic>>.SuccessResult(result);
    }
}

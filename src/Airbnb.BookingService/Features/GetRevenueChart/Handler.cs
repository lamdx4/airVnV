using Microsoft.EntityFrameworkCore;
using Airbnb.BookingService.Infrastructure;
using Airbnb.BookingService.Domain;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.BookingService.Features.GetRevenueChart;

public sealed class Handler(BookingDbContext db)
    : Mediator.IQueryHandler<Request, ApiResponse<List<RevenueChartPoint>>>
{
    public async ValueTask<ApiResponse<List<RevenueChartPoint>>> Handle(Request req, CancellationToken ct)
    {
        var days = Math.Clamp(req.Days, 1, 365);
        var startDate = DateTimeOffset.UtcNow.AddDays(-days).Date;

        var bookings = await db.Bookings.AsNoTracking()
            .Where(b => b.Status == BookingStatus.Confirmed && b.CreatedAt >= startDate)
            .GroupBy(b => b.CreatedAt.Date)
            .Select(g => new
            {
                Date = g.Key,
                Revenue = g.Sum(b => b.TotalPrice),
                Bookings = g.Count()
            })
            .OrderBy(x => x.Date)
            .ToListAsync(ct);

        var result = Enumerable.Range(0, days)
            .Select(offset => startDate.AddDays(offset))
            .Select(date =>
            {
                var entry = bookings.FirstOrDefault(b => b.Date == date);
                return new RevenueChartPoint(
                    date.ToString("yyyy-MM-dd"),
                    entry?.Revenue ?? 0,
                    entry?.Bookings ?? 0
                );
            })
            .ToList();

        return ApiResponse<List<RevenueChartPoint>>.SuccessResult(result);
    }
}

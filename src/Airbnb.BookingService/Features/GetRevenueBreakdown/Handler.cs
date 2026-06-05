using Microsoft.EntityFrameworkCore;
using Airbnb.BookingService.Infrastructure;
using Airbnb.BookingService.Domain;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.BookingService.Features.GetRevenueBreakdown;

public sealed class Handler(BookingDbContext db)
    : Mediator.IQueryHandler<Request, ApiResponse<List<RevenueBreakdownPoint>>>
{
    public async ValueTask<ApiResponse<List<RevenueBreakdownPoint>>> Handle(Request req, CancellationToken ct)
    {
        var groupBy = NormalizeGroupBy(req.GroupBy);

        var fromStart = req.From.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var toEnd = req.To.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);

        // Confirmed bookings in range (for revenue + bookings per period)
        var confirmed = await db.Bookings.AsNoTracking()
            .Where(b => b.Status == BookingStatus.Confirmed)
            .Where(b => b.CreatedAt >= fromStart && b.CreatedAt <= toEnd)
            .Select(b => new { b.CreatedAt, b.TotalPrice })
            .ToListAsync(ct);

        // Cancelled bookings in range (for cancellations per period)
        var cancelled = await db.Bookings.AsNoTracking()
            .Where(b => b.Status == BookingStatus.Cancelled)
            .Where(b => b.CreatedAt >= fromStart && b.CreatedAt <= toEnd)
            .Select(b => b.CreatedAt)
            .ToListAsync(ct);

        // Group by period
        var confirmedByPeriod = confirmed
            .GroupBy(b => FormatPeriod(b.CreatedAt, groupBy))
            .ToDictionary(g => g.Key, g => (Revenue: g.Sum(x => x.TotalPrice), Bookings: g.Count()));

        var cancelledByPeriod = cancelled
            .GroupBy(d => FormatPeriod(d, groupBy))
            .ToDictionary(g => g.Key, g => g.Count());

        // Union all periods (so cancelled-only periods appear too)
        var allPeriods = confirmedByPeriod.Keys
            .Union(cancelledByPeriod.Keys)
            .OrderBy(p => p)
            .ToList();

        var result = allPeriods.Select(period => new RevenueBreakdownPoint(
            Period: period,
            Revenue: confirmedByPeriod.TryGetValue(period, out var c) ? c.Revenue : 0m,
            Bookings: confirmedByPeriod.TryGetValue(period, out var c2) ? c2.Bookings : 0,
            Cancellations: cancelledByPeriod.TryGetValue(period, out var cn) ? cn : 0
        )).ToList();

        return ApiResponse<List<RevenueBreakdownPoint>>.SuccessResult(result);
    }

    private static string NormalizeGroupBy(string groupBy) =>
        groupBy?.ToLowerInvariant() switch
        {
            "week" => "week",
            "month" => "month",
            _ => "day"
        };

    private static string FormatPeriod(DateTimeOffset createdAt, string groupBy) => groupBy switch
    {
        "week" => $"W{System.Globalization.ISOWeek.GetWeekOfYear(createdAt.UtcDateTime)}-{createdAt.UtcDateTime.Year}",
        "month" => createdAt.UtcDateTime.ToString("yyyy-MM"),
        _ => createdAt.UtcDateTime.ToString("yyyy-MM-dd")
    };
}

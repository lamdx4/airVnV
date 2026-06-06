using FastEndpoints;
using Airbnb.ServiceDefaults.Infrastructure;
using Airbnb.UserService.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.UserService.Features.Admin.GetUserGrowthReport;

public class Endpoint(UserDbContext db) : Endpoint<Request, ApiResponse<List<Response>>>
{
    public override void Configure()
    {
        Get("/user-growth");
        Group<ReportsGroup>();
        Roles("Admin", "Moderator");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var fromDate = DateOnly.Parse(req.From);
        var toDate = DateOnly.Parse(req.To);

        var fromStart = fromDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var toEnd = toDate.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);

        var baselineTotal = await db.Users.AsNoTracking()
            .Where(u => u.CreatedAt < fromStart)
            .CountAsync(ct);

        var rawCounts = await db.Users.AsNoTracking()
            .Where(u => u.CreatedAt >= fromStart && u.CreatedAt <= toEnd)
            .Select(u => u.CreatedAt)
            .ToListAsync(ct);

        var groupBy = (req.GroupBy ?? "day").ToLowerInvariant();
        var buckets = ReportBucketing.GenerateBuckets(fromDate, toDate, groupBy);

        var grouped = rawCounts
            .GroupBy(d => ReportBucketing.BucketKey(DateOnly.FromDateTime(d), groupBy))
            .ToDictionary(g => g.Key, g => g.Count());

        var running = baselineTotal;
        var result = new List<Response>(buckets.Count);
        foreach (var (key, label) in buckets)
        {
            grouped.TryGetValue(key, out var newUsers);
            running += newUsers;
            result.Add(new Response(label, newUsers, running));
        }

        Response = ApiResponse<List<Response>>.SuccessResult(result, "User growth retrieved");
    }
}

internal static class ReportBucketing
{
    public static List<(string Key, string Label)> GenerateBuckets(DateOnly from, DateOnly to, string groupBy)
    {
        var list = new List<(string, string)>();
        var cursor = NormalizeStart(from, groupBy);
        while (cursor <= to)
        {
            var key = BucketKey(cursor, groupBy);
            var label = BucketLabel(cursor, groupBy);
            list.Add((key, label));
            cursor = Advance(cursor, groupBy);
        }
        return list;
    }

    public static string BucketKey(DateOnly d, string groupBy) => groupBy switch
    {
        "month" => $"{d.Year:D4}-{d.Month:D2}",
        "week" => $"{ISOWeekYear(d):D4}-W{ISOWeekNumber(d):D2}",
        _ => d.ToString("yyyy-MM-dd")
    };

    private static string BucketLabel(DateOnly d, string groupBy) => groupBy switch
    {
        "month" => d.ToString("yyyy-MM"),
        "week" => $"W{ISOWeekNumber(d):D2} {d:yyyy}",
        _ => d.ToString("MM-dd")
    };

    private static DateOnly NormalizeStart(DateOnly d, string groupBy) => groupBy switch
    {
        "month" => new DateOnly(d.Year, d.Month, 1),
        "week" => d.AddDays(-((int)d.DayOfWeek == 0 ? 6 : (int)d.DayOfWeek - 1)),
        _ => d
    };

    private static DateOnly Advance(DateOnly d, string groupBy) => groupBy switch
    {
        "month" => d.AddMonths(1),
        "week" => d.AddDays(7),
        _ => d.AddDays(1)
    };

    private static int ISOWeekNumber(DateOnly d) =>
        System.Globalization.ISOWeek.GetWeekOfYear(d.ToDateTime(TimeOnly.MinValue));

    private static int ISOWeekYear(DateOnly d) =>
        System.Globalization.ISOWeek.GetYear(d.ToDateTime(TimeOnly.MinValue));
}

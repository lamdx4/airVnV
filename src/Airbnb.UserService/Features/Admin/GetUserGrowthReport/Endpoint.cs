using FastEndpoints;
using Airbnb.ServiceDefaults.Infrastructure;
using Airbnb.SharedKernel.Reports;
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

        var bucket = BucketStrategyFactory.For(req.GroupBy);
        var buckets = bucket.GenerateBuckets(fromDate, toDate);

        var grouped = rawCounts
            .GroupBy(d => bucket.Key(DateOnly.FromDateTime(d)))
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

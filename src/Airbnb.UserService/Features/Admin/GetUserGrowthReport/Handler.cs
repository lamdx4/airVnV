using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.SharedKernel.Reports;
using Airbnb.UserService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Admin.GetUserGrowthReport;

public sealed class GetUserGrowthReportHandler(UserDbContext db)
    : IQueryHandler<Request, List<Response>>
{
    public async ValueTask<List<Response>> Handle(Request req, CancellationToken ct)
    {
        if (!DateOnly.TryParse(req.From, out var fromDate) || !DateOnly.TryParse(req.To, out var toDate))
            throw new BusinessException("Invalid date range. Use yyyy-MM-dd.", "INVALID_DATE_RANGE");

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

        return result;
    }
}

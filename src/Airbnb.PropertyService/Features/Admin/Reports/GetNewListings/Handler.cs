using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.PropertyService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;
using Airbnb.SharedKernel.Reports;

namespace Airbnb.PropertyService.Features.Admin.Reports.GetNewListings;

public sealed class GetNewListingsHandler(AppDbContext db)
    : IQueryHandler<Request, List<Response>>
{
    public async ValueTask<List<Response>> Handle(Request req, CancellationToken ct)
    {
        if (!DateOnly.TryParse(req.From, out var from) || !DateOnly.TryParse(req.To, out var to))
            throw new BusinessException("Invalid date range. Use yyyy-MM-dd.", "INVALID_DATE_RANGE");

        var fromStart = new DateTimeOffset(from.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
        var toEnd = new DateTimeOffset(to.ToDateTime(TimeOnly.MaxValue), TimeSpan.Zero);

        var bucket = BucketStrategyFactory.For(req.GroupBy);

        var raw = await db.Properties.AsNoTracking()
            .Where(p => p.CreatedAt >= fromStart && p.CreatedAt <= toEnd)
            .Select(p => p.CreatedAt)
            .ToListAsync(ct);

        var grouped = raw
            .GroupBy(d => bucket.Key(DateOnly.FromDateTime(d.UtcDateTime)))
            .ToDictionary(g => g.Key, g => g.Count());

        var buckets = bucket.GenerateBuckets(from, to);
        return buckets
            .Select(b => new Response(b.Label, grouped.TryGetValue(b.Key, out var c) ? c : 0))
            .ToList();
    }
}

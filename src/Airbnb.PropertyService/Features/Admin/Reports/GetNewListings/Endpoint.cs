using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Airbnb.PropertyService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;
using Airbnb.SharedKernel.Reports;

namespace Airbnb.PropertyService.Features.Admin.Reports.GetNewListings;

public record Request
{
    public string From { get; init; } = string.Empty;
    public string To { get; init; } = string.Empty;
    public string GroupBy { get; init; } = "day"; // day | week | month
}

public record Response(string Label, int NewListings);

public class Endpoint(AppDbContext db) : Endpoint<Request, ApiResponse<List<Response>>>
{
    public override void Configure()
    {
        Get("/api/properties/admin/reports/new-listings");
        AllowAnonymous();
        Summary(s => s.Summary = "Admin: new listings time series");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var from = DateOnly.Parse(req.From);
        var to = DateOnly.Parse(req.To);

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
        var result = buckets
            .Select(b => new Response(b.Label, grouped.TryGetValue(b.Key, out var c) ? c : 0))
            .ToList();

        await Send.ResponseAsync(ApiResponse<List<Response>>.SuccessResult(result), cancellation: ct);
    }
}

using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Airbnb.PropertyService.Infrastructure;
using Airbnb.PropertyService.Domain.Enums;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.Admin.Reports.GetPriceDistribution;

public record PriceBucket(string Label, decimal Min, decimal? Max, int Count);

public record Response(
    List<PriceBucket> Buckets,
    decimal Median,
    decimal P90,
    decimal Average,
    int Total
);

public class Endpoint(AppDbContext db) : EndpointWithoutRequest<ApiResponse<Response>>
{
    // Buckets in USD-per-night. Tweak as needed.
    private static readonly (decimal Min, decimal? Max, string Label)[] Buckets =
    [
        (0, 25, "$0–25"),
        (25, 50, "$25–50"),
        (50, 100, "$50–100"),
        (100, 200, "$100–200"),
        (200, 400, "$200–400"),
        (400, 800, "$400–800"),
        (800, null, "$800+"),
    ];

    public override void Configure()
    {
        Get("/api/properties/admin/reports/price-distribution");
        AllowAnonymous();
        Summary(s => s.Summary = "Admin: base price histogram for published listings");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var prices = await db.Properties.AsNoTracking()
            .Where(p => p.Status == PropertyStatus.Published)
            .Select(p => p.Pricing.BasePrice)
            .ToListAsync(ct);

        var bucketCounts = Buckets.Select(b =>
        {
            var count = prices.Count(p => p >= b.Min && (b.Max == null || p < b.Max.Value));
            return new PriceBucket(b.Label, b.Min, b.Max, count);
        }).ToList();

        decimal median = 0, p90 = 0, avg = 0;
        if (prices.Count > 0)
        {
            var sorted = prices.OrderBy(p => p).ToList();
            median = sorted[sorted.Count / 2];
            p90 = sorted[(int)Math.Floor(sorted.Count * 0.9)];
            avg = Math.Round(prices.Average(), 2);
        }

        var response = new Response(bucketCounts, median, p90, avg, prices.Count);
        await Send.ResponseAsync(ApiResponse<Response>.SuccessResult(response), cancellation: ct);
    }
}

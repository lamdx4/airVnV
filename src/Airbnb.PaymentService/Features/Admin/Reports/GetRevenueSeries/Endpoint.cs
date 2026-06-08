using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Airbnb.PaymentService.Domain;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Features.Admin.Reports.GetRevenueSeries;

public record Request
{
    public string From { get; init; } = string.Empty;
    public string To { get; init; } = string.Empty;
    public string GroupBy { get; init; } = "day";
    public string? Currency { get; init; }
}

public record RevenuePoint(string Label, decimal Gmv, decimal NetRevenue, int TransactionCount);

public class Endpoint(PaymentDbContext db) : Endpoint<Request, ApiResponse<List<RevenuePoint>>>
{
    public override void Configure()
    {
        Get("/api/admin/payments/reports/revenue-series");
        AllowAnonymous();
        Summary(s => s.Summary = "Admin: revenue time series (GMV + net revenue) by day/week/month");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var from = DateOnly.Parse(req.From);
        var to = DateOnly.Parse(req.To);
        var fromStart = new DateTimeOffset(from.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
        var toEnd = new DateTimeOffset(to.ToDateTime(TimeOnly.MaxValue), TimeSpan.Zero);
        var groupBy = (req.GroupBy ?? "day").ToLowerInvariant();
        var currencyFilter = req.Currency?.ToUpperInvariant();

        var gmvStatuses = new[] { PaymentStatus.Success, PaymentStatus.Refunded, PaymentStatus.PartiallyRefunded };

        var paymentsQuery = db.Payments.AsNoTracking()
            .Where(p => p.CreatedAt >= fromStart && p.CreatedAt <= toEnd && gmvStatuses.Contains(p.Status));
        if (currencyFilter is not null)
            paymentsQuery = paymentsQuery.Where(p => p.Currency == currencyFilter);

        var payments = await paymentsQuery
            .Select(p => new { p.CreatedAt, p.Amount })
            .ToListAsync(ct);

        var payoutsQuery = db.Payouts.AsNoTracking()
            .Where(p => p.Status == PayoutStatus.Completed && p.CompletedAt != null
                        && p.CompletedAt >= fromStart && p.CompletedAt <= toEnd);
        if (currencyFilter is not null)
            payoutsQuery = payoutsQuery.Where(p => p.Currency == currencyFilter);

        var payouts = await payoutsQuery
            .Select(p => new { p.CompletedAt, p.PlatformFee })
            .ToListAsync(ct);

        var gmvByBucket = payments
            .GroupBy(p => ReportBucketing.BucketKey(DateOnly.FromDateTime(p.CreatedAt.UtcDateTime), groupBy))
            .ToDictionary(g => g.Key, g => (Total: g.Sum(x => x.Amount), Count: g.Count()));

        var netByBucket = payouts
            .GroupBy(p => ReportBucketing.BucketKey(DateOnly.FromDateTime(p.CompletedAt!.Value.UtcDateTime), groupBy))
            .ToDictionary(g => g.Key, g => g.Sum(x => x.PlatformFee));

        var buckets = ReportBucketing.GenerateBuckets(from, to, groupBy);
        var result = buckets.Select(b =>
        {
            gmvByBucket.TryGetValue(b.Key, out var gmv);
            netByBucket.TryGetValue(b.Key, out var net);
            return new RevenuePoint(b.Label, gmv.Total, net, gmv.Count);
        }).ToList();

        await Send.ResponseAsync(ApiResponse<List<RevenuePoint>>.SuccessResult(result), cancellation: ct);
    }
}

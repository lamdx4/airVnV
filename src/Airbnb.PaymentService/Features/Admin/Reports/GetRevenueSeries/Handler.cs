using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.PaymentService.Domain;
using Airbnb.ServiceDefaults.Infrastructure;
using Airbnb.SharedKernel.Reports;

namespace Airbnb.PaymentService.Features.Admin.Reports.GetRevenueSeries;

public sealed class GetRevenueSeriesHandler(PaymentDbContext db)
    : IQueryHandler<Request, List<RevenuePoint>>
{
    public async ValueTask<List<RevenuePoint>> Handle(Request req, CancellationToken ct)
    {
        if (!DateOnly.TryParse(req.From, out var from) || !DateOnly.TryParse(req.To, out var to))
            throw new BusinessException("Invalid date range. Use yyyy-MM-dd.", "INVALID_DATE_RANGE");

        var fromStart = new DateTimeOffset(from.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
        var toEnd = new DateTimeOffset(to.ToDateTime(TimeOnly.MaxValue), TimeSpan.Zero);
        var bucket = BucketStrategyFactory.For(req.GroupBy);
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
            .GroupBy(p => bucket.Key(DateOnly.FromDateTime(p.CreatedAt.UtcDateTime)))
            .ToDictionary(g => g.Key, g => (Total: g.Sum(x => x.Amount), Count: g.Count()));

        var netByBucket = payouts
            .GroupBy(p => bucket.Key(DateOnly.FromDateTime(p.CompletedAt!.Value.UtcDateTime)))
            .ToDictionary(g => g.Key, g => g.Sum(x => x.PlatformFee));

        var buckets = bucket.GenerateBuckets(from, to);
        return buckets.Select(b =>
        {
            gmvByBucket.TryGetValue(b.Key, out var gmv);
            netByBucket.TryGetValue(b.Key, out var net);
            return new RevenuePoint(b.Label, gmv.Total, net, gmv.Count);
        }).ToList();
    }
}

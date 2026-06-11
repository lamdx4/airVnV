using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.PaymentService.Domain;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Features.Admin.Reports.GetRevenueOverview;

public sealed class GetRevenueOverviewHandler(PaymentDbContext db)
    : IQueryHandler<Request, Response>
{
    public async ValueTask<Response> Handle(Request req, CancellationToken ct)
    {
        if (!DateOnly.TryParse(req.From, out var from) || !DateOnly.TryParse(req.To, out var to))
            throw new BusinessException("Invalid date range. Use yyyy-MM-dd.", "INVALID_DATE_RANGE");

        var fromStart = new DateTimeOffset(from.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
        var toEnd = new DateTimeOffset(to.ToDateTime(TimeOnly.MaxValue), TimeSpan.Zero);

        var gmvStatuses = new[] { PaymentStatus.Success, PaymentStatus.Refunded, PaymentStatus.PartiallyRefunded };
        var refundedStatuses = new[] { PaymentStatus.Refunded, PaymentStatus.PartiallyRefunded };

        var gmvByCurrency = await db.Payments.AsNoTracking()
            .Where(p => p.CreatedAt >= fromStart && p.CreatedAt <= toEnd && gmvStatuses.Contains(p.Status))
            .GroupBy(p => p.Currency)
            .Select(g => new CurrencyAmount(g.Key, g.Sum(p => p.Amount)))
            .ToListAsync(ct);

        var successCount = await db.Payments.AsNoTracking()
            .CountAsync(p => p.CreatedAt >= fromStart && p.CreatedAt <= toEnd && p.Status == PaymentStatus.Success, ct);

        var refundedCount = await db.Payments.AsNoTracking()
            .CountAsync(p => p.CreatedAt >= fromStart && p.CreatedAt <= toEnd && refundedStatuses.Contains(p.Status), ct);

        var netByCurrency = await db.Payouts.AsNoTracking()
            .Where(p => p.Status == PayoutStatus.Completed
                        && p.CompletedAt != null
                        && p.CompletedAt >= fromStart && p.CompletedAt <= toEnd)
            .GroupBy(p => p.Currency)
            .Select(g => new CurrencyAmount(g.Key, g.Sum(p => p.PlatformFee)))
            .ToListAsync(ct);

        return new Response(gmvByCurrency, netByCurrency, successCount, refundedCount);
    }
}

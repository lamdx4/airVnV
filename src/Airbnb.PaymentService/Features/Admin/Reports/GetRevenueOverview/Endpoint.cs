using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Airbnb.PaymentService.Domain;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Features.Admin.Reports.GetRevenueOverview;

public record Request
{
    public string From { get; init; } = string.Empty;
    public string To { get; init; } = string.Empty;
}

public record CurrencyAmount(string Currency, decimal Amount);

public record Response(
    List<CurrencyAmount> Gmv,
    List<CurrencyAmount> NetRevenue,
    int SuccessCount,
    int RefundedCount
);

public class Endpoint(PaymentDbContext db) : Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Get("/api/admin/payments/reports/revenue-overview");
        AllowAnonymous();
        Summary(s => s.Summary = "Admin: revenue overview (GMV, net revenue) within a date range");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var from = DateOnly.Parse(req.From);
        var to = DateOnly.Parse(req.To);
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

        var response = new Response(gmvByCurrency, netByCurrency, successCount, refundedCount);
        await Send.ResponseAsync(ApiResponse<Response>.SuccessResult(response), cancellation: ct);
    }
}

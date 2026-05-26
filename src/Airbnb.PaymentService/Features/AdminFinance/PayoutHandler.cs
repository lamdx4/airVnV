using Airbnb.PaymentService.Domain;
using Airbnb.PaymentService.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.PaymentService.Features.AdminFinance;

public class Handler(PaymentDbContext dbContext)
{
    public async Task<PayoutResponse> Handle(PayoutRequest request, CancellationToken ct)
    {
        var query = dbContext.Payments.AsNoTracking();

        // Apply date filters
        if (request.FromDate.HasValue)
            query = query.Where(p => p.CreatedAt >= request.FromDate.Value);
        if (request.ToDate.HasValue)
            query = query.Where(p => p.CreatedAt <= request.ToDate.Value);

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new PayoutItem
            {
                PayoutId = p.Id,
                HostId = p.BookingId, // Placeholder - should be joined with BookingService
                HostName = "Pending", // Placeholder
                Amount = p.Amount,
                Currency = p.Currency,
                Status = p.Status.ToString(),
                BookingCount = 1,
                CreatedAt = p.CreatedAt,
                ProcessedAt = p.Status == PaymentStatus.Success ? p.CreatedAt : null
            })
            .ToListAsync(ct);

        return new PayoutResponse
        {
            Items = items,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

    public async Task<DashboardFinanceResponse> GetDashboardStats(DashboardRequest request, CancellationToken ct)
    {
        var fromDate = request.FromDate ?? DateTimeOffset.UtcNow.AddDays(-30);
        var toDate = request.ToDate ?? DateTimeOffset.UtcNow;

        var periodPayments = dbContext.Payments
            .AsNoTracking()
            .Where(p => p.CreatedAt >= fromDate && p.CreatedAt <= toDate);

        // Calculate financials
        var totalPayIn = await periodPayments
            .Where(p => p.Status == PaymentStatus.Success)
            .SumAsync(p => p.Amount, ct);

        var totalPayOut = totalPayIn * 0.9m; // Placeholder - would need actual payout records
        var platformRevenue = totalPayIn * 0.1m; // Placeholder - 10% platform fee

        // Pending payouts count
        var pendingPayments = await periodPayments
            .Where(p => p.Status == PaymentStatus.Success)
            .CountAsync(ct);

        var averageTransaction = pendingPayments > 0 ? totalPayIn / pendingPayments : 0;

        // Daily stats for chart
        var dailyStats = await periodPayments
            .Where(p => p.Status == PaymentStatus.Success)
            .GroupBy(p => p.CreatedAt.Date)
            .Select(g => new DailyFinanceStat
            {
                Date = g.Key.ToString("yyyy-MM-dd"),
                PayIn = g.Sum(p => p.Amount),
                PayOut = g.Sum(p => p.Amount) * 0.9m,
                Revenue = g.Sum(p => p.Amount) * 0.1m
            })
            .OrderBy(x => x.Date)
            .Take(30)
            .ToListAsync(ct);

        return new DashboardFinanceResponse
        {
            TotalPayIn = totalPayIn,
            TotalPayOut = totalPayOut,
            PlatformRevenue = platformRevenue,
            PendingPayouts = totalPayOut,
            PendingPayoutCount = pendingPayments,
            AverageTransactionAmount = averageTransaction,
            DailyStats = dailyStats
        };
    }
}

using Airbnb.PaymentService.Domain;
using Airbnb.PaymentService.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.PaymentService.Features.TransactionHistory;

public class Handler(PaymentDbContext dbContext)
{
    private const decimal PlatformFeeRate = 0.10m; // 10% platform fee

    public async Task<Response> Handle(Request request, CancellationToken ct)
    {
        var query = dbContext.Payments.AsNoTracking();

        // Apply filters
        if (!string.IsNullOrEmpty(request.TransactionId))
            query = query.Where(p => p.TransactionId != null && p.TransactionId.Contains(request.TransactionId));

        if (!string.IsNullOrEmpty(request.Status) && Enum.TryParse<PaymentStatus>(request.Status, true, out var status))
            query = query.Where(p => p.Status == status);

        if (!string.IsNullOrEmpty(request.Currency))
            query = query.Where(p => p.Currency == request.Currency.ToUpperInvariant());

        if (request.MinAmount.HasValue)
            query = query.Where(p => p.Amount >= request.MinAmount.Value);

        if (request.MaxAmount.HasValue)
            query = query.Where(p => p.Amount <= request.MaxAmount.Value);

        if (request.FromDate.HasValue)
            query = query.Where(p => p.CreatedAt >= request.FromDate.Value);

        if (request.ToDate.HasValue)
            query = query.Where(p => p.CreatedAt <= request.ToDate.Value);

        // Sorting
        query = request.SortBy.ToLowerInvariant() switch
        {
            "amount" => request.SortOrder == "asc" ? query.OrderBy(p => p.Amount) : query.OrderByDescending(p => p.Amount),
            "status" => request.SortOrder == "asc" ? query.OrderBy(p => p.Status) : query.OrderByDescending(p => p.Status),
            _ => request.SortOrder == "asc" ? query.OrderBy(p => p.CreatedAt) : query.OrderByDescending(p => p.CreatedAt)
        };

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new TransactionItem
            {
                PaymentId = p.Id,
                BookingId = p.BookingId,
                TransactionId = p.TransactionId,
                Amount = p.Amount,
                Currency = p.Currency,
                Status = p.Status.ToString(),
                Type = "PayIn", // Default to PayIn for now
                PlatformFee = p.Amount * PlatformFeeRate,
                NetAmount = p.Amount * (1 - PlatformFeeRate),
                CreatedAt = p.CreatedAt,
                ProcessedAt = p.Status == PaymentStatus.Success ? p.CreatedAt : null
            })
            .ToListAsync(ct);

        // Get summary stats (separate query for performance)
        var allPayments = dbContext.Payments.AsNoTracking();
        if (request.FromDate.HasValue) allPayments = allPayments.Where(p => p.CreatedAt >= request.FromDate.Value);
        if (request.ToDate.HasValue) allPayments = allPayments.Where(p => p.CreatedAt <= request.ToDate.Value);

        var summary = new TransactionSummary
        {
            TotalPayIn = await allPayments.Where(p => p.Status == PaymentStatus.Success).SumAsync(p => p.Amount, ct),
            TotalPayOut = await allPayments.Where(p => p.Status == PaymentStatus.Success).SumAsync(p => p.Amount * 0.9m, ct),
            TotalTransactions = await allPayments.CountAsync(ct),
            SuccessTransactions = await allPayments.CountAsync(p => p.Status == PaymentStatus.Success, ct),
            FailedTransactions = await allPayments.CountAsync(p => p.Status == PaymentStatus.Failed, ct),
            PendingTransactions = await allPayments.CountAsync(p => p.Status == PaymentStatus.Pending, ct)
        };

        return new Response
        {
            Items = items,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize,
            Summary = summary
        };
    }
}

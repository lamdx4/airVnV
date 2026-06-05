using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.PaymentService.Domain;
using Airbnb.PaymentService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Features.GetAdminPayments;

public sealed class Handler(PaymentDbContext db) : IQueryHandler<Request, PagedResponse<AdminPaymentResponse>>
{
    public async ValueTask<PagedResponse<AdminPaymentResponse>> Handle(Request req, CancellationToken ct)
    {
        var query = db.Payments.AsNoTracking();

        // Apply search filter (transaction ID)
        if (!string.IsNullOrWhiteSpace(req.Search))
        {
            var searchTerm = $"%{req.Search}%";
            query = query.Where(p =>
                (p.TransactionId != null && EF.Functions.ILike(p.TransactionId, searchTerm)));
        }

        // Apply status filter
        if (req.Status.HasValue)
        {
            query = query.Where(p => (int)p.Status == req.Status.Value);
        }

        // Apply date range filter
        if (!string.IsNullOrWhiteSpace(req.FromDate) && DateTimeOffset.TryParse(req.FromDate, out var fromDate))
        {
            query = query.Where(p => p.CreatedAt >= fromDate);
        }
        if (!string.IsNullOrWhiteSpace(req.ToDate) && DateTimeOffset.TryParse(req.ToDate, out var toDate))
        {
            query = query.Where(p => p.CreatedAt <= toDate);
        }

        var totalCount = await query.CountAsync(ct);

        // Apply sorting
        query = (req.SortBy?.ToLowerInvariant(), req.SortOrder?.ToLowerInvariant()) switch
        {
            ("amount", "asc") => query.OrderBy(p => p.Amount),
            ("amount", "desc") => query.OrderByDescending(p => p.Amount),
            ("createdat", "asc") => query.OrderBy(p => p.CreatedAt),
            ("status", _) => query.OrderBy(p => p.Status),
            _ => query.OrderByDescending(p => p.CreatedAt),
        };

        var items = await query
            .Skip((req.PageNumber - 1) * req.PageSize)
            .Take(req.PageSize)
            .Select(p => new AdminPaymentResponse(
                p.Id,
                p.BookingId,
                p.Amount,
                p.Currency,
                (int)p.Status,
                p.TransactionId,
                "VNPay",
                (string)null!,
                (string)null!,
                p.CreatedAt,
                p.ExpiresAt
            ))
            .ToListAsync(ct);

        return new PagedResponse<AdminPaymentResponse>(items, totalCount, req.PageNumber, req.PageSize);
    }
}

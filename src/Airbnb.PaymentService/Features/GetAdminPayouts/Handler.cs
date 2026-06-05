using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.PaymentService.Domain;
using Airbnb.PaymentService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Features.GetAdminPayouts;

public sealed class Handler(PaymentDbContext db) : IQueryHandler<Request, PagedResponse<AdminPayoutResponse>>
{
    public async ValueTask<PagedResponse<AdminPayoutResponse>> Handle(Request req, CancellationToken ct)
    {
        var query = db.Payouts.AsNoTracking();

        if (req.Status.HasValue)
            query = query.Where(p => (int)p.Status == req.Status.Value);

        var totalCount = await query.CountAsync(ct);

        query = (req.SortBy?.ToLowerInvariant(), req.SortOrder?.ToLowerInvariant()) switch
        {
            ("payoutamount", "asc") => query.OrderBy(p => p.PayoutAmount),
            ("payoutamount", "desc") => query.OrderByDescending(p => p.PayoutAmount),
            ("createdat", "asc") => query.OrderBy(p => p.CreatedAt),
            _ => query.OrderByDescending(p => p.CreatedAt),
        };

        var items = await query
            .Skip((req.PageNumber - 1) * req.PageSize)
            .Take(req.PageSize)
            .Select(p => new AdminPayoutResponse(
                p.Id,
                p.HostId,
                p.TotalEarnings,
                p.PlatformFee,
                p.PayoutAmount,
                p.Currency,
                (int)p.Status,
                p.ItemCount,
                p.ApprovedAt,
                p.CompletedAt,
                p.CreatedAt
            ))
            .ToListAsync(ct);

        return new PagedResponse<AdminPayoutResponse>(items, totalCount, req.PageNumber, req.PageSize);
    }
}

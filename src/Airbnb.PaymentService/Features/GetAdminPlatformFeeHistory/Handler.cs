using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.PaymentService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Features.GetAdminPlatformFeeHistory;

public sealed class Handler(PaymentDbContext db) : IQueryHandler<Request, PagedResponse<PlatformFeeHistoryItem>>
{
    public async ValueTask<PagedResponse<PlatformFeeHistoryItem>> Handle(Request req, CancellationToken ct)
    {
        var query = db.PlatformFeeConfigs
            .AsNoTracking()
            .OrderByDescending(c => c.CreatedAt);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((req.PageNumber - 1) * req.PageSize)
            .Take(req.PageSize)
            .Select(c => new PlatformFeeHistoryItem(
                c.Id,
                c.FeePercentage,
                c.Description,
                c.IsActive,
                c.ChangedBy,
                c.PreviousValue,
                c.CreatedAt
            ))
            .ToListAsync(ct);

        return new PagedResponse<PlatformFeeHistoryItem>(items, totalCount, req.PageNumber, req.PageSize);
    }
}

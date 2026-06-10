using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.PaymentService.Domain;
using Airbnb.PaymentService.Infrastructure.HttpClients;

namespace Airbnb.PaymentService.Features.Admin.GetAdminPayouts;

public sealed class GetAdminPayoutsHandler(PaymentDbContext db, UserServiceClient userClient)
    : IQueryHandler<Request, PagedResponse<AdminPayoutItem>>
{
    public async ValueTask<PagedResponse<AdminPayoutItem>> Handle(Request req, CancellationToken ct)
    {
        var page = req.Page < 1 ? 1 : req.Page;
        var pageSize = req.PageSize is < 1 or > 100 ? 20 : req.PageSize;

        var query = db.Payouts.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(req.Status) &&
            Enum.TryParse<PayoutStatus>(req.Status, ignoreCase: true, out var status))
        {
            query = query.Where(p => p.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(req.HostId) && Guid.TryParse(req.HostId, out var hostId))
        {
            query = query.Where(p => p.HostId == hostId);
        }

        if (!string.IsNullOrWhiteSpace(req.Currency))
        {
            var c = req.Currency.ToUpperInvariant();
            query = query.Where(p => p.Currency == c);
        }

        query = query.OrderByDescending(p => p.CreatedAt);

        var total = await query.CountAsync(ct);

        var rows = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new {
                p.Id, p.HostId, p.TotalEarnings, p.PlatformFee, p.PayoutAmount,
                p.Currency, p.Status, p.ItemCount, p.CreatedAt, p.ApprovedAt, p.CompletedAt
            })
            .ToListAsync(ct);

        var hostInfos = await userClient.GetBasicInfosAsync(rows.Select(r => r.HostId), ct);

        var items = rows.Select(p => {
            hostInfos.TryGetValue(p.HostId, out var u);
            return new AdminPayoutItem(
                p.Id, p.HostId, u?.FullName, u?.Email, u?.AvatarUrl,
                p.TotalEarnings, p.PlatformFee, p.PayoutAmount,
                p.Currency, p.Status, p.ItemCount, p.CreatedAt, p.ApprovedAt, p.CompletedAt
            );
        }).ToList();

        return new PagedResponse<AdminPayoutItem>(items, total, page, pageSize);
    }
}

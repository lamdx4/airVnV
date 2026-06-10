using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.PaymentService.Domain;
using Airbnb.PaymentService.Infrastructure.HttpClients;

namespace Airbnb.PaymentService.Features.Admin.GetHostBalances;

public sealed class GetHostBalancesHandler(PaymentDbContext db, UserServiceClient userClient)
    : IQueryHandler<Request, Response>
{
    public async ValueTask<Response> Handle(Request req, CancellationToken ct)
    {
        var page = req.Page < 1 ? 1 : req.Page;
        var pageSize = req.PageSize is < 1 or > 100 ? 20 : req.PageSize;

        var query = db.HostBalances.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(req.Currency))
        {
            var c = req.Currency.ToUpperInvariant();
            query = query.Where(b => b.Currency == c);
        }

        if (!string.IsNullOrWhiteSpace(req.HostId) && Guid.TryParse(req.HostId, out var hostId))
        {
            query = query.Where(b => b.HostId == hostId);
        }

        query = query.OrderByDescending(b => b.PendingBalance + b.AvailableBalance);
        var total = await query.CountAsync(ct);

        var rows = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(b => new {
                b.Id, b.HostId, b.Currency,
                b.PendingBalance, b.AvailableBalance, b.UpdatedAt
            })
            .ToListAsync(ct);

        var hostInfos = await userClient.GetBasicInfosAsync(rows.Select(r => r.HostId), ct);

        var items = rows.Select(b => {
            hostInfos.TryGetValue(b.HostId, out var u);
            return new HostBalanceItem(
                b.Id, b.HostId,
                u?.FullName, u?.Email, u?.AvatarUrl,
                b.Currency,
                b.PendingBalance, b.AvailableBalance,
                b.PendingBalance + b.AvailableBalance,
                b.UpdatedAt);
        }).ToList();

        var totals = await db.HostBalances.AsNoTracking()
            .GroupBy(b => b.Currency)
            .Select(g => new { Currency = g.Key, Total = g.Sum(x => x.PendingBalance + x.AvailableBalance) })
            .ToListAsync(ct);

        var totalVnd = totals.FirstOrDefault(t => t.Currency == "VND")?.Total ?? 0;
        var totalUsd = totals.FirstOrDefault(t => t.Currency == "USD")?.Total ?? 0;

        return new Response(
            new PagedResponse<HostBalanceItem>(items, total, page, pageSize),
            totalVnd, totalUsd
        );
    }
}

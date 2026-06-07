using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Airbnb.PaymentService.Domain;
using Airbnb.PaymentService.Infrastructure.HttpClients;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Features.Admin.GetHostBalances;

public record Request
{
    [BindFrom("page")] public int Page { get; init; } = 1;
    [BindFrom("pageSize")] public int PageSize { get; init; } = 20;
    public string? Currency { get; init; }
    public string? HostId { get; init; }
}

public record HostBalanceItem(
    Guid Id,
    Guid HostId,
    string? HostName,
    string? HostEmail,
    string? HostAvatarUrl,
    string Currency,
    decimal PendingBalance,
    decimal AvailableBalance,
    decimal TotalHeld,
    DateTimeOffset UpdatedAt
);

public record PagedResponse<T>(List<T> Items, int TotalCount, int Page, int PageSize)
{
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;
}

public record Response(
    PagedResponse<HostBalanceItem> Page,
    decimal TotalEscrowVnd,
    decimal TotalEscrowUsd
);

public class Endpoint(PaymentDbContext db, UserServiceClient userClient) : Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Get("/api/admin/host-balances");
        AllowAnonymous();
        Summary(s => s.Summary = "Admin: list host balances (platform escrow ledger)");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
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

        // Enrich with host name/email from UserService
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

        var response = new Response(
            new PagedResponse<HostBalanceItem>(items, total, page, pageSize),
            totalVnd, totalUsd
        );

        await Send.ResponseAsync(ApiResponse<Response>.SuccessResult(response), cancellation: ct);
    }
}

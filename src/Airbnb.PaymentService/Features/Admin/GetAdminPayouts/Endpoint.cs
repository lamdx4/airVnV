using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Airbnb.PaymentService.Domain;
using Airbnb.PaymentService.Infrastructure.HttpClients;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Features.Admin.GetAdminPayouts;

public record Request
{
    [BindFrom("page")] public int Page { get; init; } = 1;
    [BindFrom("pageSize")] public int PageSize { get; init; } = 20;
    public string? Status { get; init; }
    public string? HostId { get; init; }
    public string? Currency { get; init; }
}

public record AdminPayoutItem(
    Guid Id,
    Guid HostId,
    string? HostName,
    string? HostEmail,
    string? HostAvatarUrl,
    decimal TotalEarnings,
    decimal PlatformFee,
    decimal PayoutAmount,
    string Currency,
    PayoutStatus Status,
    int ItemCount,
    DateTimeOffset CreatedAt,
    DateTimeOffset? ApprovedAt,
    DateTimeOffset? CompletedAt
);

public record PagedResponse<T>(List<T> Items, int TotalCount, int Page, int PageSize)
{
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;
}

public class Endpoint(PaymentDbContext db, UserServiceClient userClient) : Endpoint<Request, ApiResponse<PagedResponse<AdminPayoutItem>>>
{
    public override void Configure()
    {
        Get("/api/admin/payouts");
        AllowAnonymous();
        Summary(s => s.Summary = "Admin: list payouts owed to hosts");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
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

        var response = new PagedResponse<AdminPayoutItem>(items, total, page, pageSize);
        await Send.ResponseAsync(ApiResponse<PagedResponse<AdminPayoutItem>>.SuccessResult(response), cancellation: ct);
    }
}

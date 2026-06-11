using FastEndpoints;

namespace Airbnb.PaymentService.Features.Admin.GetHostBalances;

public record Request : Mediator.IQuery<Response>
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

using FastEndpoints;
using Airbnb.PaymentService.Domain;

namespace Airbnb.PaymentService.Features.Admin.GetAdminPayouts;

public record Request : Mediator.IQuery<PagedResponse<AdminPayoutItem>>
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

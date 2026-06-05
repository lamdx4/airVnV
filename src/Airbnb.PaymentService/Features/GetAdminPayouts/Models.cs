using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Features.GetAdminPayouts;

public record Request(
    [property: BindFrom("page")] int PageNumber = 1,
    [property: BindFrom("pageSize")] int PageSize = 10,
    string? Search = null,
    int? Status = null,
    string? SortBy = null,
    string? SortOrder = null
) : IQuery<PagedResponse<AdminPayoutResponse>>;

public record AdminPayoutResponse(
    Guid Id,
    Guid HostId,
    decimal TotalEarnings,
    decimal PlatformFee,
    decimal PayoutAmount,
    string Currency,
    int Status,
    int ItemCount,
    DateTimeOffset? ApprovedAt,
    DateTimeOffset? CompletedAt,
    DateTimeOffset CreatedAt
);

public record PagedResponse<T>(
    List<T> Items,
    int TotalCount,
    int PageNumber,
    int PageSize
)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasNextPage => PageNumber < TotalPages;
    public bool HasPreviousPage => PageNumber > 1;
}

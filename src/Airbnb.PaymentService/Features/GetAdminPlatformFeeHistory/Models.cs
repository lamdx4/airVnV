using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Features.GetAdminPlatformFeeHistory;

public record Request(
    [property: BindFrom("page")] int PageNumber = 1,
    [property: BindFrom("pageSize")] int PageSize = 20
) : IQuery<PagedResponse<PlatformFeeHistoryItem>>;

public record PlatformFeeHistoryItem(
    Guid Id,
    decimal FeePercentage,
    string? Description,
    bool IsActive,
    Guid ChangedBy,
    decimal? PreviousValue,
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

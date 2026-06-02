using FastEndpoints;
using Mediator;
using Airbnb.PropertyService.Domain.Enums;

namespace Airbnb.PropertyService.Features.GetAdminProperties;

public record Request(
    [property: BindFrom("page")] int PageNumber = 1,
    [property: BindFrom("pageSize")] int PageSize = 10,
    string? SearchTerm = null,
    int? Status = null,
    string? SortBy = null,
    string? SortOrder = null
) : Mediator.IQuery<PagedResponse<AdminPropertyResponse>>;

public record AdminPropertyResponse(
    Guid Id,
    Guid HostId,
    string Title,
    string DisplayAddress,
    PropertyType Type,
    PropertyStatus Status,
    decimal BasePrice,
    string? CoverImageUrl,
    int GuestCount,
    int BedroomCount,
    int BathroomCount,
    decimal AverageRating,
    int ReviewCount,
    string? SuspensionReason,
    string? RejectionReason,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt
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

using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.GetMyProperties;

public record Request(
    int PageNumber = 1, 
    int PageSize = 10,
    string? SearchTerm = null,
    int? Status = null
) : Mediator.IQuery<PagedResponse<PropertyResponse>>;

public record PropertyResponse(
    Guid Id,
    string Title,
    string DisplayAddress,
    int Status,
    decimal BasePrice,
    string? CoverImageUrl,
    int GuestCount,
    int BedroomCount,
    DateTime CreatedAt,
    DateTime? UpdatedAt
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

// Internal request để mang theo RequesterId mà không lộ ra ngoài Swagger/Public API
public record InternalRequest(
    Guid RequesterId, 
    int PageNumber, 
    int PageSize,
    string? SearchTerm,
    int? Status
) : Request(PageNumber, PageSize, SearchTerm, Status);

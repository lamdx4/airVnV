namespace Airbnb.PropertyService.Features.GetReviews;

public record ReviewDto(
    Guid Id,
    Guid GuestId,
    int Rating,
    string Comment,
    DateTimeOffset CreatedAt
);

public record GetReviewsResponse(
    long TotalCount,
    int Page,
    int PageSize,
    List<ReviewDto> Reviews
);

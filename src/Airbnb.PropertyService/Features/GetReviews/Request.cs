using Mediator;

namespace Airbnb.PropertyService.Features.GetReviews;

public record GetReviewsRequest(
    Guid PropertyId,
    int Page = 1,
    int PageSize = 20
) : IQuery<GetReviewsResponse>;

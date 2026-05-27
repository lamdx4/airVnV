namespace Airbnb.PropertyService.Features.DeleteReview;

public record DeleteReviewResponse(
    Guid ReviewId,
    decimal NewAverageRating,
    int ReviewCount
);

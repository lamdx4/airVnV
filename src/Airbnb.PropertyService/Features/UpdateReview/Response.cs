namespace Airbnb.PropertyService.Features.UpdateReview;

public record UpdateReviewResponse(
    Guid ReviewId,
    decimal NewAverageRating,
    int ReviewCount
);

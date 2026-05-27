using Mediator;

namespace Airbnb.PropertyService.Features.DeleteReview;

public record DeleteReviewRequest(
    Guid PropertyId,
    Guid ReviewId
) : ICommand<DeleteReviewResponse>
{
    // Context properties set by Endpoint
    public Guid GuestId { get; set; }
}

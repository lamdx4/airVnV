using Mediator;

namespace Airbnb.PropertyService.Features.UpdateReview;

public record UpdateReviewRequest(
    Guid PropertyId,
    Guid ReviewId,
    int Rating,
    string Comment
) : ICommand<UpdateReviewResponse>
{
    // Context properties set by Endpoint
    public Guid GuestId { get; set; }
}

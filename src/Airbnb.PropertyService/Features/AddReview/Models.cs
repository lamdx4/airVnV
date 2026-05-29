using Mediator;

namespace Airbnb.PropertyService.Features.AddReview;

public record Request(
    Guid PropertyId,
    Guid BookingId,
    Guid GuestId,
    int Rating,
    string Comment
) : ICommand<Response>;

public record Response(Guid ReviewId);

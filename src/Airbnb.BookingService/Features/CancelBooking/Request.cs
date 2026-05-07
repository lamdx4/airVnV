using Mediator;

namespace Airbnb.BookingService.Features.CancelBooking;

public record Request(Guid BookingId) : ICommand
{
    [FastEndpoints.FromHeader("X-User-Id")]
    public Guid UserId { get; init; }
}

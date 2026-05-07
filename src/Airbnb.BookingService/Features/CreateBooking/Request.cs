using FastEndpoints;
using Mediator;

namespace Airbnb.BookingService.Features.CreateBooking;

public record Request(Guid PropertyId, DateOnly CheckIn, DateOnly CheckOut, int GuestCount) : Mediator.ICommand<Response>
{
    [FromHeader("X-User-Id")]
    public Guid UserId { get; init; }
}

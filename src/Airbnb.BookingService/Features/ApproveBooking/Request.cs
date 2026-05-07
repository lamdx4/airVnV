using Mediator;

namespace Airbnb.BookingService.Features.ApproveBooking;

public record Request(Guid BookingId) : ICommand
{
    [FastEndpoints.FromHeader("X-User-Id")]
    public Guid HostId { get; init; }
}

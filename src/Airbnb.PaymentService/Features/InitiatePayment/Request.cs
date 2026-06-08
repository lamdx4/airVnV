using FastEndpoints;

namespace Airbnb.PaymentService.Features.InitiatePayment;

public record Request(Guid BookingId, string? IpAddress = null) : Mediator.ICommand<Response>
{
    [FromHeader("X-User-Id")]
    public Guid UserId { get; init; }
}

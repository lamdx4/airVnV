namespace Airbnb.PaymentService.Features.InitiatePayment;

public record Request(Guid BookingId, Guid UserId, string? IpAddress = null) : Mediator.ICommand<Response>;

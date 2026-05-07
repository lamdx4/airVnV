namespace Airbnb.PaymentService.Features.FailPayment;

public record Command(Guid PaymentId, string? ErrorCode, string? Message) : Mediator.ICommand<bool>;

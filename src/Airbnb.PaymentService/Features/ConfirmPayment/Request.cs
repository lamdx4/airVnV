namespace Airbnb.PaymentService.Features.ConfirmPayment;

public record Command(Guid PaymentId, string TransactionId) : Mediator.ICommand<bool>;

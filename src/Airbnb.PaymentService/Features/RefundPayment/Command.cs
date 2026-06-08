using Mediator;

namespace Airbnb.PaymentService.Features.RefundPayment;

public record Command(
    Guid PaymentId,
    decimal Amount,        // amount to refund this round (partial supported)
    string Reason,
    Guid PerformedBy,
    Guid? TicketId = null
) : ICommand<Result>;

public record Result(
    Guid RefundId,
    Guid PaymentId,
    decimal RefundedNow,
    decimal TotalRefunded,
    bool IsFullRefund);

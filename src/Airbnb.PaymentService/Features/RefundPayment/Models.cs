using Mediator;

namespace Airbnb.PaymentService.Features.RefundPayment;

public record Command(
    Guid PaymentId,
    decimal? Amount,
    string Reason,
    Guid? TicketId
) : ICommand<RefundPaymentResponse>;

public record RefundPaymentResponse(
    Guid RefundId,
    Guid PaymentId,
    decimal RefundAmount,
    bool IsFullRefund,
    int NewPaymentStatus
);

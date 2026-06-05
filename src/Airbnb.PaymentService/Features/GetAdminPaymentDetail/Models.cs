using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Features.GetAdminPaymentDetail;

public record Request(Guid PaymentId) : IQuery<AdminPaymentDetailResponse>;

public record AdminPaymentDetailResponse(
    Guid Id,
    Guid BookingId,
    decimal Amount,
    string Currency,
    int Status,
    string? TransactionId,
    string Provider,
    string? PaymentUrl,
    DateTimeOffset? ExpiresAt,
    DateTimeOffset CreatedAt,
    List<RefundRecordDto> Refunds
);

public record RefundRecordDto(
    Guid Id,
    decimal Amount,
    string Reason,
    bool IsFullRefund,
    Guid PerformedBy,
    DateTimeOffset CreatedAt
);

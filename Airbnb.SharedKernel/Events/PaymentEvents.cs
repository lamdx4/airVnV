namespace Airbnb.SharedKernel.Events;

public record InitiatePaymentCommand(
    Guid BookingId, 
    decimal Amount, 
    string Currency, 
    string CountryCode,
    Guid UserId);

public record PaymentSucceededEvent(Guid PaymentId, Guid BookingId, decimal Amount, string Currency, string TransactionId)
{
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
}

public record PaymentFailedEvent(Guid PaymentId, Guid BookingId, string? ErrorCode)
{
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
}

public record RefundPaymentCommand(Guid BookingId, string Reason);

public record PaymentRefundedEvent(
    Guid PaymentId,
    Guid BookingId,
    decimal RefundAmount,
    string Currency,
    bool IsFullRefund,
    string Reason)
{
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
}

/// <summary>
/// Published by RefundPaymentCommandConsumer when a refund attempt is permanently failed
/// (non-retryable: already paid out, payment not found, etc.).
/// BookingStateMachine listens to this to transition Refunding → RefundFailed.
/// </summary>
public record RefundPaymentFailedEvent(Guid BookingId, string ErrorCode, string Reason)
{
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
}

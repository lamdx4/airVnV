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

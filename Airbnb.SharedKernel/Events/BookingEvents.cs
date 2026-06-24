namespace Airbnb.SharedKernel.Events;

public record BookingCreatedEvent(
    Guid BookingId, 
    Guid PropertyId, 
    Guid GuestId, 
    decimal TotalPrice, 
    string CurrencyCode, 
    string CountryCode,
    string BookingMode)
{
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
}

// Bug #2 Fix: Integration event published via Outbox after AwaitForApproval() is called
// Saga correlates on BookingId to trigger InitiatePaymentCommand
public record BookingAwaitingApprovalEvent(
    Guid BookingId,
    Guid PropertyId,
    Guid HostId,
    Guid GuestId)
{
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
}

public record BookingConfirmedEvent(Guid BookingId, Guid PropertyId, Guid UserId, decimal TotalPrice, DateTimeOffset CheckIn, DateTimeOffset CheckOut)
{
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
}

public record BookingCancelledEvent(Guid BookingId, Guid PropertyId, string Reason)
{
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
}

/// <summary>
/// Published when a confirmed booking is being cancelled and requires a refund.
/// BookingStateMachine listens to this to orchestrate the refund via RefundPaymentCommand.
/// </summary>
public record BookingRefundingEvent(Guid BookingId, Guid PropertyId, string Reason)
{
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
}

using Airbnb.SharedKernel.Domain;

namespace Airbnb.BookingService.Domain;

public record BookingCreatedDomainEvent(
    Guid BookingId, 
    Guid PropertyId, 
    Guid GuestId, 
    decimal TotalPrice, 
    string CurrencyCode, 
    string CountryCode,
    string BookingMode,
    long AggregateVersion
) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
    Guid IDomainEvent.AggregateId => BookingId;
}

public record BookingConfirmedDomainEvent(
    Guid BookingId, 
    Guid PropertyId,
    Guid GuestId,
    decimal TotalPrice,
    DateOnly CheckIn,
    DateOnly CheckOut,
    long AggregateVersion
) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
    Guid IDomainEvent.AggregateId => BookingId;
}

// Bug #7 Fix: added PropertyId and Reason — no more hardcoded Guid.Empty or string in mapper
public record BookingCancelledDomainEvent(
    Guid BookingId,
    Guid PropertyId,
    string Reason,
    long AggregateVersion
) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
    Guid IDomainEvent.AggregateId => BookingId;
}

// Bug #2 Fix: new event raised by AwaitForApproval() so Saga can trigger InitiatePaymentCommand
public record BookingAwaitingApprovalDomainEvent(
    Guid BookingId,
    Guid PropertyId,
    Guid HostId,
    Guid GuestId,
    long AggregateVersion
) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
    Guid IDomainEvent.AggregateId => BookingId;
}

/// <summary>
/// Raised when a Confirmed booking is being cancelled and a refund must be issued first.
/// The Saga orchestrates the refund before transitioning to Cancelled.
/// </summary>
public record BookingRefundingDomainEvent(
    Guid BookingId,
    Guid PropertyId,
    string Reason,
    long AggregateVersion
) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
    Guid IDomainEvent.AggregateId => BookingId;
}

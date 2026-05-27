using Airbnb.SharedKernel.Domain;

namespace Airbnb.BookingService.Domain;

public record BookingCreatedDomainEvent(
    Guid BookingId, 
    Guid PropertyId, 
    Guid GuestId, 
    decimal TotalPrice, 
    string CurrencyCode, 
    string CountryCode,
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

public record BookingCancelledDomainEvent(
    Guid BookingId, 
    long AggregateVersion
) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
    Guid IDomainEvent.AggregateId => BookingId;
}

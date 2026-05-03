using Airbnb.SharedKernel.Domain;

namespace Airbnb.PropertyService.Domain;

// ============================================================
// Domain Events – CHỈ những event có consumer thực sự
// ============================================================

/// <summary>
/// Khi được admin approve – SearchService cần index, host nhận notification.
/// </summary>
public record PropertyPublishedEvent(
    Guid PropertyId,
    Guid HostId,
    string Title,
    string CountryCode,
    string? Admin1Code,
    string? Admin2Code,
    double Latitude,
    double Longitude) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public Guid AggregateId => PropertyId;
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
}

/// <summary>
/// Khi bị admin suspend – BookingService PHẢI block future bookings ngay.
/// </summary>
public record PropertySuspendedEvent(
    Guid PropertyId,
    string Reason) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public Guid AggregateId => PropertyId;
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
}

/// <summary>
/// Khi host fix xong và được reinstate – BookingService unblock, SearchService re-index.
/// </summary>
public record PropertyReinstatedEvent(
    Guid PropertyId,
    Guid HostId) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public Guid AggregateId => PropertyId;
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
}

/// <summary>
/// Khi bị archive – BookingService cancel pending bookings, SearchService remove.
/// </summary>
public record PropertyArchivedEvent(
    Guid PropertyId) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public Guid AggregateId => PropertyId;
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
}

/// <summary>
/// Khi host submit draft để admin review – NotificationService cần báo admin.
/// </summary>
public record PropertySubmittedEvent(
    Guid PropertyId,
    Guid HostId) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public Guid AggregateId => PropertyId;
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
}

/// <summary>
/// Khi pricing thay đổi – BookingService cần recalculate pending quotes.
/// </summary>
public record PricingUpdatedEvent(
    Guid PropertyId,
    decimal NewBasePrice,
    string CurrencyCode) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public Guid AggregateId => PropertyId;
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
}

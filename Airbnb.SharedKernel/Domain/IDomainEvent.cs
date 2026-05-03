namespace Airbnb.SharedKernel.Domain;

/// <summary>
/// Marker interface cho tất cả Domain Events trong hệ thống.
/// Convention: immutable, past tense (PropertyPublished, BookingConfirmed...).
/// Domain KHÔNG biết về messaging infrastructure (không có Topic/Exchange).
/// </summary>
public interface IDomainEvent
{
    Guid EventId { get; }
    Guid AggregateId { get; }   // Dùng làm CorrelationId khi dispatch
    DateTimeOffset OccurredAt { get; }
}

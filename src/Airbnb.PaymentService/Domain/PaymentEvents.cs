using Airbnb.SharedKernel.Domain;

namespace Airbnb.PaymentService.Domain;

public record PaymentInitiatedDomainEvent(
    Guid PaymentId, 
    Guid BookingId, 
    decimal Amount, 
    string Currency, 
    long AggregateVersion
) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
    Guid IDomainEvent.AggregateId => PaymentId;
}

public record PaymentSucceededDomainEvent(
    Guid PaymentId, 
    Guid BookingId, 
    decimal Amount, 
    string Currency, 
    string TransactionId, 
    long AggregateVersion
) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
    Guid IDomainEvent.AggregateId => PaymentId;
}

public record PaymentFailedDomainEvent(
    Guid PaymentId,
    Guid BookingId,
    string? ErrorCode,
    long AggregateVersion
) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
    Guid IDomainEvent.AggregateId => PaymentId;
}

public record PaymentRefundedDomainEvent(
    Guid PaymentId,
    Guid BookingId,
    decimal RefundAmount,
    string Currency,
    bool IsFullRefund,
    string Reason,
    long AggregateVersion
) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
    Guid IDomainEvent.AggregateId => PaymentId;
}

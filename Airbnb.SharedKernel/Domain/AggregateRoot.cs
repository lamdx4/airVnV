namespace Airbnb.SharedKernel.Domain;

/// <summary>
/// Base class cho tất cả Aggregate Roots trong hệ thống.
/// Quản lý collection domain events nội bộ, expose read-only ra ngoài.
/// Service-specific Aggregate Root kế thừa class này.
/// </summary>
public abstract class AggregateRoot
{
    private readonly List<IDomainEvent> _domainEvents = new();

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void Raise(IDomainEvent @event) => _domainEvents.Add(@event);

    /// <summary>
    /// Gọi sau khi publish events sang Outbox để tránh re-dispatch.
    /// </summary>
    public void ClearDomainEvents() => _domainEvents.Clear();
}

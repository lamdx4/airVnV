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
    public long Version { get; protected set; } = 1;

    protected void Raise(IDomainEvent @event) => _domainEvents.Add(@event);

    public void ClearDomainEvents() => _domainEvents.Clear();
}

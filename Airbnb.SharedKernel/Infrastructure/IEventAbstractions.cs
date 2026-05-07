using Airbnb.SharedKernel.Domain;

namespace Airbnb.SharedKernel.Infrastructure;

/// <summary>
/// Cầu nối trích xuất Domain Events và đẩy sang Integration Events (Asynchronous).
/// Thường dùng MassTransit Outbox.
/// </summary>
public interface IIntegrationEventBridge
{
    Task StageAsync(IEnumerable<IDomainEvent> events, CancellationToken ct);
}

/// <summary>
/// Thực thi các chính sách nghiệp vụ đồng bộ (Synchronous Policies) ngay trong cùng transaction.
/// </summary>
public interface IDomainEventPolicyExecutor
{
    Task ExecuteAsync(IEnumerable<IDomainEvent> events, CancellationToken ct);
}

/// <summary>
/// Ánh xạ Domain Event sang Integration Event (POCO).
/// Phục vụ cho Native AOT bằng cách explicit mapping.
/// </summary>
public interface IIntegrationEventMapper
{
    object Map(IDomainEvent @event);
}

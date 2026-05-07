using Airbnb.SharedKernel.Domain;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.SharedKernel.Infrastructure;

/// <summary>
/// Base DbContext cho toàn bộ hệ thống.
/// Tự động quản lý quy trình: Sync Policies -> Integration Staging -> Atomic Commit -> Cleanup.
/// </summary>
public abstract class AppDbContextBase(
    DbContextOptions options,
    IIntegrationEventBridge bridge,
    IDomainEventPolicyExecutor policyExecutor) : DbContext(options)
{
    public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        // 1. PEEK: Lấy toàn bộ domain events từ ChangeTracker mà chưa xóa vội
        var aggregateRoots = ChangeTracker.Entries<AggregateRoot>()
            .Where(x => x.Entity.DomainEvents.Any())
            .Select(x => x.Entity).ToList();
        
        var domainEvents = aggregateRoots.SelectMany(x => x.DomainEvents).ToList();

        // 2. INTERNAL POLICIES (Synchronous - Same Transaction)
        // Chạy các logic nghiệp vụ nội bộ service ngay lập tức.
        await policyExecutor.ExecuteAsync(domainEvents, ct);

        // 3. INTEGRATION STAGING (Asynchronous - Outbox buffer)
        // Ánh xạ Domain Events -> Integration Events và ném vào Outbox buffer của MassTransit.
        await bridge.StageAsync(domainEvents, ct);

        // 4. ATOMIC COMMIT (Business Data + Outbox Messages)
        var result = await base.SaveChangesAsync(ct);

        // 5. CLEANUP (Memory only)
        // Chỉ dọn dẹp domain events khỏi entities khi và chỉ khi DB đã commit thành công.
        foreach (var aggregate in aggregateRoots)
        {
            aggregate.ClearDomainEvents();
        }

        return result;
    }
}

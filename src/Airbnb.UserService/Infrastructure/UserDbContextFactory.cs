using Airbnb.SharedKernel.Domain;
using Airbnb.SharedKernel.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Airbnb.UserService.Infrastructure;

/// <summary>
/// Design-time factory cho EF CLI tools (dotnet ef migrations / database update).
/// Bypasses MassTransit và Aspire hoàn toàn - chỉ dùng khi chạy CLI.
/// </summary>
public class UserDbContextFactory : IDesignTimeDbContextFactory<UserDbContext>
{
    public UserDbContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        // Fallback về connection string trực tiếp nếu Aspire chưa inject
        var connectionString = 
            config.GetConnectionString("userdb") 
            ?? "Host=localhost;Port=5433;Database=userdb;Username=postgres;Password=postgres";

        var optionsBuilder = new DbContextOptionsBuilder<UserDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        // Stub no-op implementations chỉ dùng cho design-time
        return new UserDbContext(
            optionsBuilder.Options,
            new NoOpIntegrationEventBridge(),
            new NoOpDomainEventPolicyExecutor());
    }
}

// ─── Design-time stubs (không dùng trong production) ───────────────────────

file sealed class NoOpIntegrationEventBridge : IIntegrationEventBridge
{
    public Task StageAsync(IEnumerable<IDomainEvent> events, CancellationToken ct) 
        => Task.CompletedTask;
}

file sealed class NoOpDomainEventPolicyExecutor : IDomainEventPolicyExecutor
{
    public Task ExecuteAsync(IEnumerable<IDomainEvent> events, CancellationToken ct) 
        => Task.CompletedTask;
}

using Airbnb.SharedKernel.Domain;
using Airbnb.SharedKernel.Infrastructure;
using MassTransit;

using Microsoft.Extensions.DependencyInjection;

namespace Airbnb.UserService.Infrastructure.Messaging;

public class UserIntegrationEventBridge(
    IServiceProvider serviceProvider, 
    IIntegrationEventMapper mapper) : IIntegrationEventBridge
{
    public async Task StageAsync(IEnumerable<IDomainEvent> events, CancellationToken ct)
    {
        // Giải quyết IPublishEndpoint lười biếng (lazy) để phá vỡ vòng lặp DI:
        // Bus -> Outbox -> DbContext -> Bridge -> Bus (Boom!)
        var publishEndpoint = serviceProvider.GetRequiredService<IPublishEndpoint>();

        foreach (var @event in events)
        {
            var integrationEvent = mapper.Map(@event);
            if (integrationEvent != null)
            {
                await publishEndpoint.Publish(integrationEvent, ct);
            }
        }
    }
}

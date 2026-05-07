using Airbnb.SharedKernel.Domain;
using Airbnb.SharedKernel.Infrastructure;
using MassTransit;

namespace Airbnb.UserService.Infrastructure.Messaging;

public class UserIntegrationEventBridge(
    IPublishEndpoint publishEndpoint, 
    IIntegrationEventMapper mapper) : IIntegrationEventBridge
{
    public async Task StageAsync(IEnumerable<IDomainEvent> events, CancellationToken ct)
    {
        foreach (var @event in events)
        {
            var integrationEvent = mapper.Map(@event);
            await publishEndpoint.Publish(integrationEvent, ct);
        }
    }
}

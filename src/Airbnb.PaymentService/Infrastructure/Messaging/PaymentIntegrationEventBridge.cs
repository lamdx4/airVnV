using Airbnb.SharedKernel.Domain;
using Airbnb.SharedKernel.Infrastructure;
using Airbnb.PaymentService.Domain;
using MassTransit;

namespace Airbnb.PaymentService.Infrastructure.Messaging;

public class PaymentIntegrationEventBridge(
    IPublishEndpoint publishEndpoint, 
    IIntegrationEventMapper mapper) : IIntegrationEventBridge
{
    public async Task StageAsync(IEnumerable<IDomainEvent> events, CancellationToken ct)
    {
        foreach (var @event in events)
        {
            // PaymentInitiatedDomainEvent typically doesn't need to go outside 
            // unless we have a specific integration need. 
            // Most critical are Success/Failed for the Booking Saga.
            if (@event is PaymentInitiatedDomainEvent) continue;

            var integrationEvent = mapper.Map(@event);
            await publishEndpoint.Publish(integrationEvent, ct);
        }
    }
}

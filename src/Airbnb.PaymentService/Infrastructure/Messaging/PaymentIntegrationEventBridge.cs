using Airbnb.SharedKernel.Domain;
using Airbnb.SharedKernel.Infrastructure;
using Airbnb.PaymentService.Domain;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace Airbnb.PaymentService.Infrastructure.Messaging;

public class PaymentIntegrationEventBridge(
    IServiceProvider serviceProvider,
    IIntegrationEventMapper mapper) : IIntegrationEventBridge
{
    public async Task StageAsync(IEnumerable<IDomainEvent> events, CancellationToken ct)
    {
        // Lazy-resolve IPublishEndpoint to break the DI cycle:
        // Bus -> Outbox -> PaymentDbContext -> Bridge -> Bus (Boom!)
        // Resolving at constructor time would create infinite recursion in DI
        // because UseBusOutbox() makes IPublishEndpoint depend on PaymentDbContext.
        var publishEndpoint = serviceProvider.GetRequiredService<IPublishEndpoint>();

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

using Airbnb.SharedKernel.Domain;
using Airbnb.SharedKernel.Infrastructure;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace Airbnb.BookingService.Infrastructure.Messaging;

public class BookingIntegrationEventBridge(
    IServiceProvider serviceProvider,
    IIntegrationEventMapper mapper) : IIntegrationEventBridge
{
    public async Task StageAsync(IEnumerable<IDomainEvent> events, CancellationToken ct)
    {

        var publishEndpoint = serviceProvider.GetRequiredService<IPublishEndpoint>();
        // ⚠️ MassTransit Outbox Semantics: 
        // Lệnh publishEndpoint.Publish() KHÔNG gửi bản tin đi ngay lập tức. 
        // Nó chỉ enqueue vào internal buffer của MassTransit gắn với DbContext hiện tại.
        // Thực sự insert vào database xảy ra khi SaveChangesAsync() được gọi.  
        foreach (var @event in events)
        {
            var integrationEvent = mapper.Map(@event);
            await publishEndpoint.Publish(integrationEvent, ct);
        }
    }
}

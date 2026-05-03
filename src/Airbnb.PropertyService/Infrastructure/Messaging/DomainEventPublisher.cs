using MassTransit;
using Airbnb.PropertyService.Domain;

namespace Airbnb.PropertyService.Infrastructure.Messaging;

/// <summary>
/// Bridge layer: Domain Events → MassTransit publish.
/// Domain không biết MassTransit tồn tại.
/// Topic mapping nằm hoàn toàn ở đây (Infrastructure), không leak vào Domain.
/// </summary>
public class DomainEventPublisher(IPublishEndpoint publishEndpoint)
{
    /// <summary>
    /// internal để unit test access qua InternalsVisibleTo mà không cần reflection.
    /// </summary>
    internal static readonly Dictionary<Type, string> TopicMap = new()
    {
        [typeof(PropertySubmittedEvent)]  = PropertyTopics.Submitted,
        [typeof(PropertyPublishedEvent)]  = PropertyTopics.Published,
        [typeof(PropertySuspendedEvent)]  = PropertyTopics.Suspended,
        [typeof(PropertyReinstatedEvent)] = PropertyTopics.Reinstated,
        [typeof(PropertyArchivedEvent)]   = PropertyTopics.Archived,
        [typeof(PricingUpdatedEvent)]     = PropertyTopics.PricingUpdated,
    };

    /// <summary>
    /// Dispatch tất cả domain events qua MassTransit Outbox.
    /// Gọi TRƯỚC SaveChangesAsync để MassTransit đảm bảo atomicity.
    /// </summary>
    public async Task DispatchAsync(
        IEnumerable<IDomainEvent> events,
        CancellationToken ct = default)
    {
        foreach (var @event in events)
        {
            if (!TopicMap.TryGetValue(@event.GetType(), out var topic))
                throw new InvalidOperationException(
                    $"No topic mapping for event type '{@event.GetType().Name}'. "
                    + "Add it to DomainEventPublisher.TopicMap.");

            await publishEndpoint.Publish(@event, @event.GetType(), ctx =>
            {
                ctx.Headers.Set("event-type", topic);
                ctx.CorrelationId = @event.EventId;
                ctx.SetRoutingKey(topic);
            }, ct);
        }
    }
}

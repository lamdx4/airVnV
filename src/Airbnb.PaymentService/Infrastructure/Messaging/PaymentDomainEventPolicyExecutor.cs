using Airbnb.PaymentService.Domain;
using Airbnb.SharedKernel.Domain;
using Airbnb.SharedKernel.Infrastructure;
using Mediator;

namespace Airbnb.PaymentService.Infrastructure.Messaging;

public class PaymentDomainEventPolicyExecutor(IMediator mediator) : IDomainEventPolicyExecutor
{
    public async Task ExecuteAsync(IEnumerable<IDomainEvent> events, CancellationToken ct)
    {
        foreach (var @event in events)
        {
            var notification = MapToNotification(@event);
            await mediator.Publish(notification, ct);
        }
    }

    private INotification MapToNotification(IDomainEvent @event) => @event switch
    {
        PaymentInitiatedDomainEvent e => new PaymentInitiatedNotification(e),
        PaymentSucceededDomainEvent e => new PaymentSucceededNotification(e),
        PaymentFailedDomainEvent e => new PaymentFailedNotification(e),
        PaymentRefundedDomainEvent e => new PaymentRefundedNotification(e),
        _ => throw new ArgumentException($"Unhandled domain event type: {@event.GetType().Name}")
    };
}

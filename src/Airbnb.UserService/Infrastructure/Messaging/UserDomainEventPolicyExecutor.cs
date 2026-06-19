using Airbnb.SharedKernel.Domain;
using Airbnb.SharedKernel.Infrastructure;
using Airbnb.UserService.Domain.Events;
using Mediator;

namespace Airbnb.UserService.Infrastructure.Messaging;

public class UserDomainEventPolicyExecutor(IMediator mediator) : IDomainEventPolicyExecutor
{
    public async Task ExecuteAsync(IEnumerable<IDomainEvent> events, CancellationToken ct)
    {
        foreach (var @event in events)
        {
            var notification = MapToNotification(@event);
            if (notification is not null)
                await mediator.Publish(notification, ct);
        }
    }

    private INotification? MapToNotification(IDomainEvent @event) => @event switch
    {
        UserProfileUpdatedDomainEvent e => new UserProfileUpdatedNotification(e),
        _ => null
    };
}

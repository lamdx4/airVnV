using Airbnb.SharedKernel.Domain;
using Airbnb.SharedKernel.Events;
using Airbnb.SharedKernel.Infrastructure;
using Airbnb.UserService.Domain.Events;

namespace Airbnb.UserService.Infrastructure.Messaging;

public class UserIntegrationEventMapper : IIntegrationEventMapper
{
    public object Map(IDomainEvent domainEvent) => domainEvent switch
    {
        UserProfileUpdatedDomainEvent e => new UserProfileUpdatedEvent(e.UserId, e.FullName, e.AvatarUrl),
        _ => throw new ArgumentException($"Unhandled domain event type: {domainEvent.GetType().Name}")
    };
}

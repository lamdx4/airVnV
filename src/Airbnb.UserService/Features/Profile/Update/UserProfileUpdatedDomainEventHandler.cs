using Airbnb.SharedKernel.Events;
using Airbnb.UserService.Domain.Events;
using MassTransit;
using Mediator;

namespace Airbnb.UserService.Features.Profile.Update;

public class UserProfileUpdatedDomainEventHandler(IPublishEndpoint publishEndpoint) : INotificationHandler<UserProfileUpdatedDomainEvent>
{
    public async ValueTask Handle(UserProfileUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        // Chuyển đổi Domain Event thành Integration Event để gửi ra ngoài
        var integrationEvent = new UserProfileUpdatedEvent(
            notification.AggregateId,
            notification.FullName,
            notification.AvatarUrl
        );

        // Publish event này vào MassTransit Outbox
        await publishEndpoint.Publish(integrationEvent, cancellationToken);
    }
}

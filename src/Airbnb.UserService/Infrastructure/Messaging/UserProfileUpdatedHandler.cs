using Airbnb.SharedKernel.Infrastructure;
using Mediator;

namespace Airbnb.UserService.Infrastructure.Messaging;

public sealed class UserProfileUpdatedHandler(IIntegrationEventBridge bridge) 
    : INotificationHandler<UserProfileUpdatedNotification>
{
    public async ValueTask Handle(UserProfileUpdatedNotification notification, CancellationToken ct)
    {
        // Chuyển tiếp domain event sang integration event thông qua bridge
        await bridge.StageAsync([notification.DomainEvent], ct);
    }
}

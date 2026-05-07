using Airbnb.UserService.Domain.Events;
using Mediator;

namespace Airbnb.UserService.Infrastructure.Messaging;

public record UserProfileUpdatedNotification(UserProfileUpdatedDomainEvent DomainEvent) : INotification;

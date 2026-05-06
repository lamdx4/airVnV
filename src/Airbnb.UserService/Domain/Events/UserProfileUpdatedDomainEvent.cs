using Airbnb.SharedKernel.Domain;
using Mediator;

namespace Airbnb.UserService.Domain.Events;

public record UserProfileUpdatedDomainEvent(Guid AggregateId, string FullName, string? AvatarUrl) : IDomainEvent, INotification
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
}

using Airbnb.SharedKernel.Domain;

namespace Airbnb.UserService.Domain.Events;

public record UserProfileUpdatedDomainEvent(
    Guid UserId, 
    string FullName, 
    string? AvatarUrl
) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
    Guid IDomainEvent.AggregateId => UserId;
    long IDomainEvent.AggregateVersion => 1;
}

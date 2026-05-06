namespace Airbnb.SharedKernel.Events;

public record UserProfileUpdatedEvent(Guid UserId, string DisplayName, string? AvatarUrl)
{
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
}

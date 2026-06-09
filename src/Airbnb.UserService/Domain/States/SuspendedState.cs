using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Domain.States;

internal sealed class SuspendedState : IUserStatusState
{
    public UserStatus Status => UserStatus.Suspended;

    public void Suspend(User user, string reason) =>
        throw new BusinessException("Only active users can be suspended.", "INVALID_STATUS_TRANSITION");

    public void Ban(User user, string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new BusinessException("Ban reason is required.", "REASON_REQUIRED");
        user.MarkBanned(reason);
    }

    public void Activate(User user) => user.MarkActive();
}

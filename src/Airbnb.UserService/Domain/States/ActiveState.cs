using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Domain.States;

internal sealed class ActiveState : IUserStatusState
{
    public UserStatus Status => UserStatus.Active;

    public void Suspend(User user, string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new BusinessException("Suspension reason is required.", "REASON_REQUIRED");
        user.MarkSuspended(reason);
    }

    public void Ban(User user, string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new BusinessException("Ban reason is required.", "REASON_REQUIRED");
        user.MarkBanned(reason);
    }

    public void Activate(User user) =>
        throw new BusinessException("Only suspended or banned users can be activated.", "INVALID_STATUS_TRANSITION");
}

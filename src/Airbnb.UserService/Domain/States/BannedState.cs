using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Domain.States;

internal sealed class BannedState : IUserStatusState
{
    public UserStatus Status => UserStatus.Banned;

    public void Suspend(User user, string reason) =>
        throw new BusinessException("Only active users can be suspended.", "INVALID_STATUS_TRANSITION");

    public void Ban(User user, string reason) =>
        throw new BusinessException("User is already banned.", "USER_ALREADY_BANNED");

    public void Activate(User user) => user.MarkActive();
}

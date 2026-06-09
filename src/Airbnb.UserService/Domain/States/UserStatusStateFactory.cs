namespace Airbnb.UserService.Domain.States;

internal static class UserStatusStateFactory
{
    public static IUserStatusState For(UserStatus status) => status switch
    {
        UserStatus.Active => new ActiveState(),
        UserStatus.Suspended => new SuspendedState(),
        UserStatus.Banned => new BannedState(),
        _ => throw new InvalidOperationException($"Unknown user status: {status}"),
    };
}

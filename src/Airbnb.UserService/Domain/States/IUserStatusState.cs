namespace Airbnb.UserService.Domain.States;

internal interface IUserStatusState
{
    UserStatus Status { get; }
    void Suspend(User user, string reason);
    void Ban(User user, string reason);
    void Activate(User user);
}

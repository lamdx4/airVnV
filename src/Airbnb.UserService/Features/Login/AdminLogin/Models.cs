using Airbnb.UserService.Domain;

namespace Airbnb.UserService.Features.Login.AdminLogin;

public record Request(string Email, string Password) : Mediator.ICommand<Response>;

public record Response(
    string AccessToken,
    string RefreshToken,
    string FullName,
    string Email,
    UserRole Role
);

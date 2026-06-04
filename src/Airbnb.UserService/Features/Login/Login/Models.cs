using FastEndpoints;
using Airbnb.UserService.Domain;

using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Login.Login;

public record Request(string Email, string Password) : Mediator.ICommand<ApiResponse<Response>>;

public record Response(
    string AccessToken, 
    string RefreshToken, 
    string FullName, 
    string Email, 
    UserRole Role
);

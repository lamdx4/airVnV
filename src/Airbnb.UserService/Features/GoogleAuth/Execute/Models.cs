using FastEndpoints;
using FluentValidation;
using Airbnb.UserService.Domain;

using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.GoogleAuth.Execute;

public record Request(string IdToken) : ICommand<ApiResponse<Response>>;
public record Response(string AccessToken, string RefreshToken, string FullName, string Email, UserRole Role);

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.IdToken).NotEmpty();
    }
}

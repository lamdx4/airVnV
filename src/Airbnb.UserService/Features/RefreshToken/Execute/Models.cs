using FastEndpoints;
using FluentValidation;

using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.RefreshToken.Execute;

public record Request(string RefreshToken) : ICommand<ApiResponse<Response>>;
public record Response(string AccessToken, string RefreshToken);

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}

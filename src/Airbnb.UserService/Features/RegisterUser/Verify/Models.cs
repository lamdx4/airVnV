using FastEndpoints;
using FluentValidation;

namespace Airbnb.UserService.Features.RegisterUser.Verify;

public record Request(string Email, string Code) : ICommand<Response>;
public record Response(string Message);

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Code).NotEmpty().Length(6);
    }
}

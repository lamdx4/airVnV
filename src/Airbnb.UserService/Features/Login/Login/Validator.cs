using FastEndpoints;
using FluentValidation;

namespace Airbnb.UserService.Features.Login.Login;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}

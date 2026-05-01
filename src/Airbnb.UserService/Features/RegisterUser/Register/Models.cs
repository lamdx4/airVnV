using FastEndpoints;
using FluentValidation;

namespace Airbnb.UserService.Features.RegisterUser.Register;

public record Request(
    string Email, 
    string Password, 
    string FullName
) : ICommand<Response>;

public record Response(string Message);

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(100);
    }
}

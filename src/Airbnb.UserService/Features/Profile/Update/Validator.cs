using FastEndpoints;
using FluentValidation;

namespace Airbnb.UserService.Features.Profile.Update;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(255);
        RuleFor(x => x.PhoneNumber).MaximumLength(20);
        RuleFor(x => x.Bio).MaximumLength(500);
    }
}

using FastEndpoints;
using FluentValidation;

namespace Airbnb.PropertyService.Features.AdminPropertyModeration.RejectProperty;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.PropertyId)
            .NotEmpty()
            .WithMessage("PropertyId is required.");
    }
}
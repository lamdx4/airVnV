using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;
using FastEndpoints;
using FluentValidation;

namespace Airbnb.PropertyService.Features.ManageAvailability.BlockDates;

public record Request(
    Guid PropertyId,
    DateOnly StartDate,
    DateOnly EndDate,
    string? Note
) : Mediator.ICommand<ApiResponse<bool>>
{
    [FromHeader("X-User-Id")]
    public Guid RequesterId { get; init; }
}

public class Validator : FastEndpoints.Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.PropertyId).NotEmpty();
        RuleFor(x => x.StartDate).NotEmpty();
        RuleFor(x => x.EndDate).NotEmpty().GreaterThanOrEqualTo(x => x.StartDate);
        RuleFor(x => x.Note).MaximumLength(255);
    }
}

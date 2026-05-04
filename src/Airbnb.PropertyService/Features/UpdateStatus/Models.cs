using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;
using FastEndpoints;
using FluentValidation;

namespace Airbnb.PropertyService.Features.UpdateStatus;

public record Request(Guid PropertyId, int Status) : Mediator.ICommand<ApiResponse<bool>>
{
    [FromHeader("X-User-Id")]
    public Guid RequesterId { get; init; }
}

public class Validator : FastEndpoints.Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.PropertyId).NotEmpty();
        RuleFor(x => x.Status).InclusiveBetween(0, 5); // 0: Draft, 1: Published, etc.
    }
}

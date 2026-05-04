using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;
using FastEndpoints;
using FluentValidation;

namespace Airbnb.PropertyService.Features.UpdateAmenityInfo;

public record Request(
    Guid PropertyId,
    Guid AmenityId,
    string? AdditionalInfo
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
        RuleFor(x => x.AmenityId).NotEmpty();
        RuleFor(x => x.AdditionalInfo).MaximumLength(500);
    }
}

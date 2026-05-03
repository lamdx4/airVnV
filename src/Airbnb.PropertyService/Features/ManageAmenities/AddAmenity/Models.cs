using FastEndpoints;
using Mediator;
using Microsoft.AspNetCore.Http;
using FluentValidation;

namespace Airbnb.PropertyService.Features.ManageAmenities.AddAmenity;

public record Request(Guid PropertyId, Guid AmenityId, string? AdditionalInfo) : Mediator.ICommand<Response>
{
    [FromHeader("X-User-Id")]
    public Guid RequesterId { get; init; }
}

public record Response(Guid PropertyId, Guid AmenityId, string Status);

public class Validator : FastEndpoints.Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.AmenityId).NotEmpty();
        RuleFor(x => x.AdditionalInfo).MaximumLength(255).When(x => x.AdditionalInfo is not null);
    }
}

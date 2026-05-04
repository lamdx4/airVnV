using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;
using FastEndpoints;
using FluentValidation;
using Airbnb.PropertyService.Domain.ValueObjects;

namespace Airbnb.PropertyService.Features.UpdateLocation;

public record Request(
    Guid PropertyId,
    double Latitude,
    double Longitude,
    string CountryCode,
    string DisplayAddress,
    string StreetAddress,
    string? Unit,
    string? PostalCode,
    Dictionary<string, string>? SubDivisions,
    string? Admin1Code,
    string? Admin2Code
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
        RuleFor(x => x.Latitude).InclusiveBetween(-90, 90);
        RuleFor(x => x.Longitude).InclusiveBetween(-180, 180);
        RuleFor(x => x.CountryCode).NotEmpty().Length(2);
        RuleFor(x => x.DisplayAddress).NotEmpty().MaximumLength(500);
        RuleFor(x => x.StreetAddress).NotEmpty().MaximumLength(255);
    }
}

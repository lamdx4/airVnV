using FastEndpoints;
using FluentValidation;

using Microsoft.AspNetCore.Http;

namespace Airbnb.PropertyService.Features.CreateProperty;

public record Request(
    string Name, 
    string Description, 
    decimal PricePerNight, 
    string CountryCode,
    string City,
    string? StateProvince,
    string? Ward,
    string StreetLine1,
    string? StreetLine2,
    string? PostalCode,
    double Latitude,
    double Longitude)
{
    [FromHeader("X-User-Id")]
    public Guid HostId { get; init; }
}
public record Response(Guid Id, string Name);

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Tên không được để trống").MaximumLength(255);
        RuleFor(x => x.PricePerNight).GreaterThan(0).WithMessage("Giá phải lớn hơn 0");
        RuleFor(x => x.CountryCode).NotEmpty().Length(2).WithMessage("Mã quốc gia phải có đúng 2 ký tự");
        RuleFor(x => x.City).NotEmpty();
        RuleFor(x => x.StreetLine1).NotEmpty();
        RuleFor(x => x.Latitude).InclusiveBetween(-90, 90);
        RuleFor(x => x.Longitude).InclusiveBetween(-180, 180);
    }
}

using FastEndpoints;
using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Http;
using Airbnb.PropertyService.Domain.ValueObjects;

namespace Airbnb.PropertyService.Features.CreateProperty;

// ============================================================
// Request = Command (implements ICommand → Mediator dispatch)
// ============================================================

public record Request(
    string Title,
    string Description,
    string Slug,
    // Pricing
    decimal BasePrice,
    string CurrencyCode,
    decimal CleaningFee,
    decimal ServiceFee,
    decimal WeekendPremiumPercent,
    // Location (geo)
    double Latitude,
    double Longitude,
    // Classification
    string CountryCode,
    string? Admin1Code,
    string? Admin2Code,
    // Display
    string DisplayAddress,
    // AddressRaw
    string StreetAddress,
    string? Unit,
    string? PostalCode,
    // Capacity
    int GuestCount,
    int BedroomCount,
    int BedCount,
    int BathroomCount,
    // HouseRules
    bool AllowPets,
    bool AllowSmoking,
    bool AllowEvents,
    TimeOnly CheckInTime,
    TimeOnly CheckOutTime,
    bool FlexibleCheckOut = false) : Mediator.ICommand<Response>
{
    [FromHeader("X-User-Id")]
    public Guid HostId { get; init; }
}

// ============================================================
// Response
// ============================================================

public record Response(Guid Id, string Slug);

// ============================================================
// Validator (FastEndpoints auto-discovers)
// ============================================================

public class Validator : FastEndpoints.Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(300)
            .Matches("^[a-z0-9-]+$").WithMessage("Slug chỉ được chứa chữ thường, số và dấu gạch ngang.");
        RuleFor(x => x.BasePrice).GreaterThan(0);
        RuleFor(x => x.CurrencyCode).NotEmpty().Length(3)
            .WithMessage("Currency phải theo chuẩn ISO 4217 (VND, USD...).");
        RuleFor(x => x.CountryCode).NotEmpty().Length(2)
            .WithMessage("CountryCode phải theo chuẩn ISO 3166-1 alpha-2.");
        RuleFor(x => x.Latitude).InclusiveBetween(-90, 90);
        RuleFor(x => x.Longitude).InclusiveBetween(-180, 180);
        RuleFor(x => x.DisplayAddress).NotEmpty().MaximumLength(500);
        RuleFor(x => x.StreetAddress).NotEmpty().MaximumLength(255);
        RuleFor(x => x.GuestCount).GreaterThan(0);
        RuleFor(x => x.WeekendPremiumPercent).GreaterThanOrEqualTo(0);
    }
}

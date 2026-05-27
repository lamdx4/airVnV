using FastEndpoints;
using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using Airbnb.PropertyService.Domain.Enums;

namespace Airbnb.PropertyService.Features.CreateProperty;

// ============================================================
// DTO for JSON Deserialization from Payload
// ============================================================

public record CreatePropertyDto(
    string Title,
    string Description,
    string Slug,
    decimal BasePrice,
    string CurrencyCode,
    decimal CleaningFee,
    decimal ServiceFee,
    decimal WeekendPremiumPercent,
    double Latitude,
    double Longitude,
    string CountryCode,
    string? Admin1Code,
    string? Admin2Code,
    string DisplayAddress,
    string StreetAddress,
    string? Unit,
    string? PostalCode,
    Dictionary<string, string>? SubDivisions,
    int GuestCount,
    int BedroomCount,
    int BedCount,
    int BathroomCount,
    bool AllowPets,
    bool AllowSmoking,
    bool AllowEvents,
    TimeOnly CheckInTime,
    TimeOnly CheckOutTime,
    bool FlexibleCheckOut = false,
    List<string>? CustomRules = null,
    List<Guid>? AmenityIds = null,
    BookingMode BookingMode = BookingMode.InstantBook);

// ============================================================
// HTTP Request (Multipart/form-data)
// ============================================================

public class Request 
{
    [BindFrom("Payload")]
    public string Payload { get; init; } = default!;

    [BindFrom("Images")]
    public List<IFormFile> Images { get; init; } = new();
}

// ============================================================
// Command for Mediator
// ============================================================

public record CreatePropertyCommand(
    CreatePropertyDto Data,
    List<IFormFile> Images,
    Guid HostId) : Mediator.ICommand<Response>;

// ============================================================
// Response
// ============================================================

public record Response(Guid Id, string Slug);

// ============================================================
// Validator (Validates the HTTP Request directly before Handler)
// ============================================================

public class Validator : FastEndpoints.Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Payload).NotEmpty().WithMessage("Payload string is required.");
        
        RuleFor(x => x.Images).NotEmpty().WithMessage("At least 5 images are required to create a property.");
        RuleFor(x => x.Images.Count).GreaterThanOrEqualTo(5).WithMessage("At least 5 images are required to create a property.");
        
        RuleForEach(x => x.Images).ChildRules(file => {
            file.RuleFor(x => x.Length).LessThanOrEqualTo(5 * 1024 * 1024)
                .WithMessage("Each file size must be less than 5MB.");
            file.RuleFor(x => x.ContentType).Must(x => x is "image/jpeg" or "image/png" or "image/webp")
                .WithMessage("Only JPEG, PNG and WebP images are allowed.");
        });
    }
}

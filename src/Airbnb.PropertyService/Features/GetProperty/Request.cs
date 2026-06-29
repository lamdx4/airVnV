using Airbnb.PropertyService.Domain.Enums;
using Airbnb.PropertyService.Domain.ValueObjects;
using Airbnb.PropertyService.Domain.Entities;
using Mediator;

namespace Airbnb.PropertyService.Features.GetProperty;

public record PropertyImageDto(Guid Id, string Url, ImageType Type, int DisplayOrder);
public record PropertyAmenityDto(Guid AmenityId, string Name, string IconCode, string Category, string? AdditionalInfo);
public record PropertyAvailabilityDto(Guid Id, DateOnly StartDate, DateOnly EndDate, AvailabilityType Type, string? Note);

public record PropertyDto(
    Guid Id,
    Guid HostId,
    string Title,
    string Description,
    string Slug,
    PropertyType Type,
    PropertyStatus Status,
    string BookingMode,
    double Latitude,
    double Longitude,
    string DisplayAddress,
    string CountryCode,
    string StreetAddress,
    System.Collections.Generic.Dictionary<string, string>? SubDivisions,
    Pricing Pricing,
    PropertyCapacity Capacity,
    HouseRules HouseRules,
    List<PropertyImageDto> Images,
    List<PropertyAmenityDto> PropertyAmenities,
    List<PropertyAvailabilityDto> Availabilities,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt
);

public record Request(Guid PropertyId) : IQuery<PropertyDto>;

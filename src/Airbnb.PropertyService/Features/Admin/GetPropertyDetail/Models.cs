namespace Airbnb.PropertyService.Features.Admin.GetPropertyDetail;

public record AdminPropertyImageDto(string Url, int DisplayOrder);

public record AdminPropertyAmenityDto(string Name, string Category);

public record AdminPropertyDetailDto(
    Guid Id,
    Guid HostId,
    string Title,
    string Description,
    string DisplayAddress,
    string StreetAddress,
    int Type,
    int Status,
    decimal BasePrice,
    int MaxGuests,
    int Bedrooms,
    int Bathrooms,
    decimal AverageRating,
    int ReviewCount,
    string? CoverImageUrl,
    List<AdminPropertyImageDto> Images,
    List<AdminPropertyAmenityDto> Amenities,
    string? RejectionReason,
    string? SuspensionReason,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt
);

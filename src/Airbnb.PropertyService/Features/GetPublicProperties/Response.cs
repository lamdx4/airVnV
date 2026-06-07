namespace Airbnb.PropertyService.Features.GetPublicProperties;

public record Response(
    Guid Id,
    string Title,
    string DisplayAddress,
    decimal Price,
    string Currency,
    decimal Rating,
    List<string> Images
);

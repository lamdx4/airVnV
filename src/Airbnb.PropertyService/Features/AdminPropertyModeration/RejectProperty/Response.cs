namespace Airbnb.PropertyService.Features.AdminPropertyModeration.RejectProperty;

public record Response(
    Guid PropertyId,
    string Status,
    DateTimeOffset ActionAt,
    string AdminId
);
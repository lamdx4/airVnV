namespace Airbnb.PropertyService.Features.AdminPropertyModeration.GetPendingProperties;

public record PropertySummaryDto(
    Guid PropertyId,
    string Title,
    string ThumbnailUrl,
    string HostName,
    DateTimeOffset SubmittedAt,
    string Status
);

public record Response(
    IReadOnlyList<PropertySummaryDto> Items,
    int TotalCount,
    int Page,
    int PageSize
);
namespace Airbnb.SearchService.Features.SearchProperties;

public record PagedResponse<T>(
    long TotalCount,
    int Page,
    int PageSize,
    List<T> Results
);

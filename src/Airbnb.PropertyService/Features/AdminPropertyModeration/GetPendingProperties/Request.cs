using Mediator;

namespace Airbnb.PropertyService.Features.AdminPropertyModeration.GetPendingProperties;

public record Request(
    int Page = 1,
    int PageSize = 20,
    string? Search = null,
    string? SortBy = "SubmittedAt",
    string? SortOrder = "desc"
) : IQuery<Response>;
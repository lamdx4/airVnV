using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.GetRecentActivity;

public record Request([property: BindFrom("limit")] int Limit = 10) : Mediator.IQuery<ApiResponse<List<ActivityItem>>>;

public record ActivityItem(
    Guid Id,
    string Type,
    string Description,
    DateTimeOffset Timestamp
);

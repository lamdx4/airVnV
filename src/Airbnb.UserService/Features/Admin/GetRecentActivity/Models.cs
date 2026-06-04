using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Admin.GetRecentActivity;

public record Request([property: BindFrom("limit")] int Limit = 10) : Mediator.IQuery<ApiResponse<List<ActivityItem>>>;

public record ActivityItem(
    string Id,
    string Type,
    string Description,
    string Timestamp
);

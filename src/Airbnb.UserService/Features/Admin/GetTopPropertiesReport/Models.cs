using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Admin.GetTopPropertiesReport;

public record Request(
    [property: BindFrom("from")] DateOnly From,
    [property: BindFrom("to")] DateOnly To,
    [property: BindFrom("limit")] int Limit = 10
) : Mediator.IQuery<ApiResponse<List<TopPropertyItem>>>;

public record TopPropertyItem(
    Guid Id,
    string Title,
    decimal Revenue,
    int Bookings,
    decimal OccupancyRate
);

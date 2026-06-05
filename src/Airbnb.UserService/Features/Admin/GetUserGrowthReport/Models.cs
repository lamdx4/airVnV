using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Admin.GetUserGrowthReport;

public record Request(
    [property: BindFrom("from")] DateOnly From,
    [property: BindFrom("to")] DateOnly To,
    [property: BindFrom("groupBy")] string GroupBy = "day"
) : Mediator.IQuery<ApiResponse<List<UserGrowthPoint>>>;

public record UserGrowthPoint(
    string Date,
    int Guests,
    int Hosts
);

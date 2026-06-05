using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Admin.GetRevenueBreakdownReport;

public record Request(
    [property: BindFrom("from")] DateOnly From,
    [property: BindFrom("to")] DateOnly To,
    [property: BindFrom("groupBy")] string GroupBy = "day"
) : Mediator.IQuery<ApiResponse<List<RevenueBreakdownItem>>>;

public record RevenueBreakdownItem(
    string Period,
    decimal Revenue,
    int Bookings,
    int Cancellations
);

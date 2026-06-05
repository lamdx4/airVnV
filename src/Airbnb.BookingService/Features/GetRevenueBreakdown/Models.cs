using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.BookingService.Features.GetRevenueBreakdown;

public record Request(
    [property: BindFrom("from")] DateOnly From,
    [property: BindFrom("to")] DateOnly To,
    [property: BindFrom("groupBy")] string GroupBy = "day"
) : Mediator.IQuery<ApiResponse<List<RevenueBreakdownPoint>>>;

public record RevenueBreakdownPoint(
    string Period,
    decimal Revenue,
    int Bookings,
    int Cancellations
);

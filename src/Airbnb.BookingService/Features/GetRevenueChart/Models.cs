using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.BookingService.Features.GetRevenueChart;

public record Request([property: BindFrom("days")] int Days = 30) : Mediator.IQuery<ApiResponse<List<RevenueChartPoint>>>;

public record RevenueChartPoint(
    string Date,
    decimal Revenue,
    int Bookings
);

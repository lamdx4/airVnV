using System.Net.Http.Json;
using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Admin.GetRevenueChart;

public record Request([property: BindFrom("days")] int Days = 30) : Mediator.IQuery<ApiResponse<List<RevenueChartPoint>>>;

public record RevenueChartPoint(
    string Date,
    decimal Revenue,
    int Bookings
);

using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Admin.GetReportSummary;

public record Request(
    [property: BindFrom("from")] DateOnly From,
    [property: BindFrom("to")] DateOnly To
) : Mediator.IQuery<ApiResponse<ReportSummaryResponse>>;

public record ReportSummaryResponse(
    decimal TotalRevenue,
    int TotalBookings,
    decimal AverageBookingValue,
    decimal OccupancyRate,
    int NewUsers,
    int NewProperties
);

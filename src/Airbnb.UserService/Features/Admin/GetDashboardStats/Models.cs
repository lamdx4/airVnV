using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Admin.GetDashboardStats;

public record Request : Mediator.IQuery<ApiResponse<DashboardStatsResponse>>;

public record DashboardStatsResponse(
    int TotalProperties,
    int TotalBookings,
    int TotalUsers,
    decimal TotalRevenue,
    int PendingReviews,
    int ActiveBookings
);

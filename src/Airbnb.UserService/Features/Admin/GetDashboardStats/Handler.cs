using Microsoft.EntityFrameworkCore;
using Airbnb.UserService.Infrastructure;
using Airbnb.UserService.Infrastructure.HttpClients;
using Airbnb.ServiceDefaults.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Airbnb.UserService.Features.Admin.GetDashboardStats;

public sealed class Handler(
    UserDbContext db,
    PropertyServiceClient propertyClient,
    BookingServiceClient bookingClient,
    ILogger<Handler> logger)
    : Mediator.IQueryHandler<Request, ApiResponse<DashboardStatsResponse>>
{
    public async ValueTask<ApiResponse<DashboardStatsResponse>> Handle(Request req, CancellationToken ct)
    {
        var totalUsers = await db.Users.AsNoTracking().CountAsync(ct);

        PropertyStatsResponse? propertyStats = null;
        BookingStatsResponse? bookingStats = null;

        try
        {
            propertyStats = await propertyClient.GetStatsAsync(ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to retrieve property stats from PropertyService");
        }

        try
        {
            bookingStats = await bookingClient.GetStatsAsync(ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to retrieve booking stats from BookingService");
        }

        if (propertyStats is null || bookingStats is null)
        {
            logger.LogWarning(
                "Dashboard stats unavailable — PropertyService={PropertyAvailable}, BookingService={BookingAvailable}",
                propertyStats is not null, bookingStats is not null);
            return ApiResponse<DashboardStatsResponse>.FailureResult(
                "DASHBOARD_UNAVAILABLE",
                "Some services are temporarily unavailable. Please try again later.");
        }

        return ApiResponse<DashboardStatsResponse>.SuccessResult(
            new DashboardStatsResponse(
                TotalProperties: propertyStats.TotalProperties,
                TotalBookings: bookingStats.TotalBookings,
                TotalUsers: totalUsers,
                TotalRevenue: bookingStats.TotalRevenue,
                PendingReviews: propertyStats.TotalReviews,
                ActiveBookings: bookingStats.ActiveBookings
            )
        );
    }
}

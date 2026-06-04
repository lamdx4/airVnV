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

        return ApiResponse<DashboardStatsResponse>.SuccessResult(
            new DashboardStatsResponse(
                TotalProperties: propertyStats?.TotalProperties ?? 0,
                TotalBookings: bookingStats?.TotalBookings ?? 0,
                TotalUsers: totalUsers,
                TotalRevenue: bookingStats?.TotalRevenue ?? 0,
                PendingReviews: propertyStats?.TotalReviews ?? 0,
                ActiveBookings: bookingStats?.ActiveBookings ?? 0
            )
        );
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Airbnb.UserService.Infrastructure;
using Airbnb.UserService.Infrastructure.HttpClients;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Admin.GetReportSummary;

public sealed class Handler(
    UserDbContext db,
    BookingServiceClient bookingClient,
    PropertyServiceClient propertyClient,
    ILogger<Handler> logger)
    : Mediator.IQueryHandler<Request, ApiResponse<ReportSummaryResponse>>
{
    public async ValueTask<ApiResponse<ReportSummaryResponse>> Handle(Request req, CancellationToken ct)
    {
        // Booking-derived
        BookingSummaryResponse? bookingSummary = null;
        OccupancyMetricsResponse? occupancy = null;
        int publishedCount = 0;
        int newProperties = 0;
        int newUsers = 0;

        try
        {
            bookingSummary = await bookingClient.GetSummaryAsync(req.From, req.To, ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to retrieve booking summary from BookingService");
        }

        try
        {
            occupancy = await bookingClient.GetOccupancyMetricsAsync(req.From, req.To, ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to retrieve occupancy metrics from BookingService");
        }

        try
        {
            publishedCount = await propertyClient.GetPublishedCountAsync(ct) ?? 0;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to retrieve published count from PropertyService");
        }

        try
        {
            newProperties = await propertyClient.GetNewPropertiesCountAsync(req.From, req.To, ct) ?? 0;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to retrieve new properties count from PropertyService");
        }

        // Local: new users in range
        try
        {
            var fromStart = req.From.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
            var toEnd = req.To.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);
            newUsers = await db.Users.AsNoTracking()
                .Where(u => u.CreatedAt >= fromStart && u.CreatedAt <= toEnd)
                .CountAsync(ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to count new users");
        }

        // Occupancy rate = BookedNights / (PublishedCount × RangeDays), capped at 1.0
        decimal occupancyRate = 0m;
        if (occupancy is not null && occupancy.RangeDays > 0 && publishedCount > 0)
        {
            var denominator = (decimal)occupancy.RangeDays * publishedCount;
            occupancyRate = Math.Min(1m, Math.Round(occupancy.BookedNights / denominator, 4));
        }

        return ApiResponse<ReportSummaryResponse>.SuccessResult(
            new ReportSummaryResponse(
                TotalRevenue: bookingSummary?.TotalRevenue ?? 0m,
                TotalBookings: bookingSummary?.TotalBookings ?? 0,
                AverageBookingValue: bookingSummary?.AverageBookingValue ?? 0m,
                OccupancyRate: occupancyRate,
                NewUsers: newUsers,
                NewProperties: newProperties
            )
        );
    }
}

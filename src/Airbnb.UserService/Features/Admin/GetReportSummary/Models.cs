namespace Airbnb.UserService.Features.Admin.GetReportSummary;

public record Request : Mediator.IQuery<Response>
{
    public string From { get; init; } = string.Empty;
    public string To { get; init; } = string.Empty;
}

public record Response(
    decimal TotalRevenue,
    int TotalBookings,
    decimal AverageBookingValue,
    double OccupancyRate,
    int NewUsers,
    int NewProperties,
    int TotalUsers,
    int ActiveUsers,
    int SuspendedUsers,
    int BannedUsers,
    int UserCount,
    int AdminCount
);

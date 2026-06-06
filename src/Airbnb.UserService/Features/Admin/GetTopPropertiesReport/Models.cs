namespace Airbnb.UserService.Features.Admin.GetTopPropertiesReport;

public record Request
{
    public string From { get; init; } = string.Empty;
    public string To { get; init; } = string.Empty;
    public int Limit { get; init; } = 10;
}

public record Response(
    string Id,
    string Title,
    decimal Revenue,
    int Bookings,
    double OccupancyRate
);

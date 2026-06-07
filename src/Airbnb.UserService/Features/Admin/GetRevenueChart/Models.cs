namespace Airbnb.UserService.Features.Admin.GetRevenueChart;

public record Request
{
    public string From { get; init; } = string.Empty;
    public string To { get; init; } = string.Empty;
    public string GroupBy { get; init; } = "day";
}

public record Response(
    string Label,
    decimal Revenue,
    int Bookings
);

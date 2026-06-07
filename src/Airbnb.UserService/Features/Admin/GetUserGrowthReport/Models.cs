namespace Airbnb.UserService.Features.Admin.GetUserGrowthReport;

public record Request
{
    public string From { get; init; } = string.Empty;
    public string To { get; init; } = string.Empty;
    public string GroupBy { get; init; } = "day"; // day | week | month
}

public record Response(
    string Label,
    int NewUsers,
    int TotalUsers
);

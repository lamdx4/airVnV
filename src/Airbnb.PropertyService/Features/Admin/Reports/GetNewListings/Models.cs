namespace Airbnb.PropertyService.Features.Admin.Reports.GetNewListings;

public record Request : Mediator.IQuery<List<Response>>
{
    public string From { get; init; } = string.Empty;
    public string To { get; init; } = string.Empty;
    public string GroupBy { get; init; } = "day"; // day | week | month
}

public record Response(string Label, int NewListings);

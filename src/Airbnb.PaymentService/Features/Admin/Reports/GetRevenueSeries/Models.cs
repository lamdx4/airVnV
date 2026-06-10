namespace Airbnb.PaymentService.Features.Admin.Reports.GetRevenueSeries;

public record Request : Mediator.IQuery<List<RevenuePoint>>
{
    public string From { get; init; } = string.Empty;
    public string To { get; init; } = string.Empty;
    public string GroupBy { get; init; } = "day";
    public string? Currency { get; init; }
}

public record RevenuePoint(string Label, decimal Gmv, decimal NetRevenue, int TransactionCount);

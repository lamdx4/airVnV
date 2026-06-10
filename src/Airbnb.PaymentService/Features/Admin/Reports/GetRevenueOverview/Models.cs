namespace Airbnb.PaymentService.Features.Admin.Reports.GetRevenueOverview;

public record Request : Mediator.IQuery<Response>
{
    public string From { get; init; } = string.Empty;
    public string To { get; init; } = string.Empty;
}

public record CurrencyAmount(string Currency, decimal Amount);

public record Response(
    List<CurrencyAmount> Gmv,
    List<CurrencyAmount> NetRevenue,
    int SuccessCount,
    int RefundedCount
);

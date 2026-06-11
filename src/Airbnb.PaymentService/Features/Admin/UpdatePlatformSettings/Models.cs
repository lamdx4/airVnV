namespace Airbnb.PaymentService.Features.Admin.UpdatePlatformSettings;

public record Request : Mediator.ICommand<Response>
{
    public decimal PlatformFeePercent { get; init; }
    public decimal MinPayoutAmount { get; init; }
    public string DefaultCurrency { get; init; } = "USD";
    public string? Actor { get; init; }
}

public record Response(
    Guid Id,
    decimal PlatformFeePercent,
    decimal MinPayoutAmount,
    string DefaultCurrency,
    DateTimeOffset UpdatedAt,
    string? UpdatedBy
);

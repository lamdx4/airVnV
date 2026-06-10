namespace Airbnb.PaymentService.Features.Admin.GetPlatformSettings;

public record Request : Mediator.IQuery<Response>;

public record Response(
    Guid Id,
    decimal PlatformFeePercent,
    decimal MinPayoutAmount,
    string DefaultCurrency,
    DateTimeOffset UpdatedAt,
    string? UpdatedBy
);

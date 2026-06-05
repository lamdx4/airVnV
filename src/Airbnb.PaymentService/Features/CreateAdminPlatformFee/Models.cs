using Mediator;
using Airbnb.PaymentService.Domain;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Features.CreateAdminPlatformFee;

public record Command(
    decimal FeePercentage,
    string? Description,
    Guid ChangedBy
) : ICommand<PlatformFeeCreateResponse>;

public record PlatformFeeCreateResponse(
    Guid Id,
    decimal FeePercentage,
    string? Description,
    decimal? PreviousValue,
    DateTimeOffset CreatedAt
);

using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Features.GetAdminPlatformFeeCurrent;

public record Request : IQuery<PlatformFeeCurrentResponse>;

public record PlatformFeeCurrentResponse(
    Guid Id,
    decimal FeePercentage,
    string? Description,
    Guid ChangedBy,
    decimal? PreviousValue,
    DateTimeOffset CreatedAt
);

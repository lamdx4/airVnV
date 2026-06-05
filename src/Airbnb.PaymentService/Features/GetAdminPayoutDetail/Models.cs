using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Features.GetAdminPayoutDetail;

public record Request(Guid PayoutId) : IQuery<AdminPayoutDetailResponse>;

public record AdminPayoutDetailResponse(
    Guid Id,
    Guid HostId,
    decimal TotalEarnings,
    decimal PlatformFee,
    decimal PayoutAmount,
    string Currency,
    int Status,
    int ItemCount,
    Guid? ApprovedBy,
    DateTimeOffset? ApprovedAt,
    DateTimeOffset? CompletedAt,
    DateTimeOffset CreatedAt,
    List<PayoutItemDto> Items
);

public record PayoutItemDto(
    Guid Id,
    Guid BookingId,
    Guid PaymentId,
    decimal BookingTotal,
    decimal ServiceFee,
    decimal HostEarning,
    DateOnly CheckIn,
    DateOnly CheckOut,
    string PropertyTitle,
    string GuestName
);

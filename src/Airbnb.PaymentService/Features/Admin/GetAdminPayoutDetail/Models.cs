using Airbnb.PaymentService.Domain;

namespace Airbnb.PaymentService.Features.Admin.GetAdminPayoutDetail;

public record Request : Mediator.IQuery<Response>
{
    public Guid Id { get; init; }
}

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

public record Response(
    Guid Id,
    Guid HostId,
    string? HostName,
    string? HostEmail,
    string? HostAvatarUrl,
    decimal TotalEarnings,
    decimal PlatformFee,
    decimal PayoutAmount,
    string Currency,
    PayoutStatus Status,
    int ItemCount,
    DateTimeOffset CreatedAt,
    Guid? ApprovedBy,
    DateTimeOffset? ApprovedAt,
    DateTimeOffset? CompletedAt,
    List<PayoutItemDto> Items
);

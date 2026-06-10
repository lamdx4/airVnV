using Airbnb.PaymentService.Domain;

namespace Airbnb.PaymentService.Features.Admin.GetAdminPaymentDetail;

public record Request : Mediator.IQuery<Response>
{
    public Guid Id { get; init; }
}

public record Response(
    Guid Id,
    Guid BookingId,
    Guid? GuestId,
    string? GuestName,
    string? GuestEmail,
    string? GuestAvatarUrl,
    decimal Amount,
    string Currency,
    PaymentStatus Status,
    string? TransactionId,
    string? PaymentUrl,
    DateTimeOffset CreatedAt,
    DateTimeOffset? ExpiresAt
);

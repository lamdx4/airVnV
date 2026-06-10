using Airbnb.PaymentService.Domain;

namespace Airbnb.PaymentService.Features.Admin.GetHostBalanceDetail;

public record Request : Mediator.IQuery<Response>
{
    public Guid Id { get; init; }
}

public record EntryDto(
    Guid Id,
    BalanceEntryType Type,
    decimal PendingDelta,
    decimal AvailableDelta,
    Guid? PaymentId,
    Guid? PayoutId,
    Guid? BookingId,
    string? Note,
    DateTimeOffset CreatedAt
);

public record Response(
    Guid Id,
    Guid HostId,
    string? HostName,
    string? HostEmail,
    string? HostAvatarUrl,
    string Currency,
    decimal PendingBalance,
    decimal AvailableBalance,
    DateTimeOffset UpdatedAt,
    List<EntryDto> Entries
);

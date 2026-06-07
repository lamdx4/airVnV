using System.Text.Json.Serialization;
using Airbnb.SharedKernel.Domain;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Domain;

[JsonConverter(typeof(JsonStringEnumConverter<BalanceEntryType>))]
public enum BalanceEntryType
{
    PaymentReceived,    // Guest pay-in: tăng PendingBalance
    BookingCheckedOut,  // Booking xong: chuyển Pending → Available
    PayoutApproved,     // Admin approve payout: trừ Available
    Refund,             // Hoàn tiền cho guest: trừ Pending hoặc Available
    Adjustment,         // Manual adjustment by admin
}

/// <summary>
/// Snapshot số dư của một host theo currency. Là tổng hợp từ BalanceEntries.
/// "Tiền platform đang giữ hộ host" — chia làm 2 ngăn:
///  - Pending: guest đã trả, nhưng booking chưa check-out (vẫn có thể refund)
///  - Available: booking đã check-out, sẵn sàng payout
/// </summary>
public class HostBalance : AggregateRoot
{
    public Guid Id { get; private set; }
    public Guid HostId { get; private set; }
    public string Currency { get; private set; } = default!;
    public decimal PendingBalance { get; private set; }
    public decimal AvailableBalance { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    private HostBalance() { } // EF

    public static HostBalance Create(Guid hostId, string currency) => new()
    {
        Id = Guid.CreateVersion7(),
        HostId = hostId,
        Currency = currency.ToUpperInvariant(),
        PendingBalance = 0,
        AvailableBalance = 0,
        UpdatedAt = DateTimeOffset.UtcNow,
    };

    public void ApplyEntry(BalanceEntry entry)
    {
        if (entry.Currency != Currency)
            throw new BusinessException("Currency mismatch.", "BALANCE_CURRENCY_MISMATCH");

        PendingBalance += entry.PendingDelta;
        AvailableBalance += entry.AvailableDelta;
        UpdatedAt = DateTimeOffset.UtcNow;

        if (PendingBalance < 0)
            throw new BusinessException("Pending balance cannot go negative.", "BALANCE_PENDING_NEGATIVE");
        if (AvailableBalance < 0)
            throw new BusinessException("Available balance cannot go negative.", "BALANCE_AVAILABLE_NEGATIVE");
    }

    /// <summary>Recompute balance từ list entries (đối soát).</summary>
    public void Recompute(IEnumerable<BalanceEntry> entries)
    {
        PendingBalance = entries.Where(e => e.Currency == Currency).Sum(e => e.PendingDelta);
        AvailableBalance = entries.Where(e => e.Currency == Currency).Sum(e => e.AvailableDelta);
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}

/// <summary>
/// Immutable ledger entry. Mỗi sự kiện làm thay đổi balance đều ghi 1 row.
/// Cộng dồn delta của tất cả entries cùng host+currency = balance hiện tại.
/// </summary>
public class BalanceEntry
{
    public Guid Id { get; private set; }
    public Guid HostId { get; private set; }
    public string Currency { get; private set; } = default!;
    public BalanceEntryType Type { get; private set; }
    public decimal PendingDelta { get; private set; }
    public decimal AvailableDelta { get; private set; }
    public Guid? PaymentId { get; private set; }
    public Guid? PayoutId { get; private set; }
    public Guid? BookingId { get; private set; }
    public string? Note { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private BalanceEntry() { }

    public static BalanceEntry PaymentReceived(Guid hostId, decimal hostPortion, string currency, Guid paymentId, Guid bookingId)
        => new()
        {
            Id = Guid.CreateVersion7(),
            HostId = hostId,
            Currency = currency.ToUpperInvariant(),
            Type = BalanceEntryType.PaymentReceived,
            PendingDelta = hostPortion,
            AvailableDelta = 0,
            PaymentId = paymentId,
            BookingId = bookingId,
            Note = "Guest payment received — held in pending until check-out.",
            CreatedAt = DateTimeOffset.UtcNow,
        };

    public static BalanceEntry BookingCheckedOut(Guid hostId, decimal hostPortion, string currency, Guid paymentId, Guid bookingId)
        => new()
        {
            Id = Guid.CreateVersion7(),
            HostId = hostId,
            Currency = currency.ToUpperInvariant(),
            Type = BalanceEntryType.BookingCheckedOut,
            PendingDelta = -hostPortion,
            AvailableDelta = hostPortion,
            PaymentId = paymentId,
            BookingId = bookingId,
            Note = "Booking checked-out — funds released to host's available balance.",
            CreatedAt = DateTimeOffset.UtcNow,
        };

    public static BalanceEntry PayoutApproved(Guid hostId, decimal amount, string currency, Guid payoutId)
        => new()
        {
            Id = Guid.CreateVersion7(),
            HostId = hostId,
            Currency = currency.ToUpperInvariant(),
            Type = BalanceEntryType.PayoutApproved,
            PendingDelta = 0,
            AvailableDelta = -amount,
            PayoutId = payoutId,
            Note = "Payout approved — funds disbursed to host's bank account.",
            CreatedAt = DateTimeOffset.UtcNow,
        };
}

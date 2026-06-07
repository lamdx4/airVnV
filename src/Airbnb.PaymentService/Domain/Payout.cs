using System.Text.Json.Serialization;
using Airbnb.SharedKernel.Domain;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Domain;

[JsonConverter(typeof(JsonStringEnumConverter<PayoutStatus>))]
public enum PayoutStatus
{
    Pending,    // Ready to be approved by admin
    Approved,   // Approved, queued for bank transfer
    Processing, // Bank transfer in progress
    Completed,  // Money delivered to host
    Failed,     // Transfer failed
    Cancelled,  // Admin cancelled
}

/// <summary>
/// Aggregates pay-outs owed to a host across one or more completed bookings.
/// Created by the admin (or background job) once host has eligible earnings.
/// </summary>
public class Payout : AggregateRoot
{
    public Guid Id { get; private set; }
    public Guid HostId { get; private set; }
    public decimal TotalEarnings { get; private set; } // Sum of HostEarning across items (pre-platform-fee already deducted)
    public decimal PlatformFee { get; private set; }   // Sum of platform fees withheld
    public decimal PayoutAmount { get; private set; }  // Amount actually to be transferred (= TotalEarnings)
    public string Currency { get; private set; } = default!;
    public PayoutStatus Status { get; private set; }
    public int ItemCount { get; private set; }
    public Guid? ApprovedBy { get; private set; }
    public DateTimeOffset? ApprovedAt { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private readonly List<PayoutItem> _items = new();
    public IReadOnlyCollection<PayoutItem> Items => _items.AsReadOnly();

    private Payout() { } // EF

    public static Payout Create(Guid hostId, string currency, IReadOnlyList<PayoutItem> items)
    {
        if (hostId == Guid.Empty)
            throw new BusinessException("HostId is required.", "PAYOUT_HOST_REQUIRED");
        if (items.Count == 0)
            throw new BusinessException("Payout must contain at least one item.", "PAYOUT_EMPTY");
        if (string.IsNullOrWhiteSpace(currency) || currency.Length != 3)
            throw new BusinessException("Currency must be a 3-letter ISO code.", "PAYOUT_CURRENCY_INVALID");

        var payout = new Payout
        {
            Id = Guid.CreateVersion7(),
            HostId = hostId,
            Currency = currency.ToUpperInvariant(),
            Status = PayoutStatus.Pending,
            ItemCount = items.Count,
            CreatedAt = DateTimeOffset.UtcNow,
            TotalEarnings = items.Sum(i => i.HostEarning),
            PlatformFee = items.Sum(i => i.ServiceFee),
            PayoutAmount = items.Sum(i => i.HostEarning),
        };
        payout._items.AddRange(items);
        return payout;
    }

    public void Approve(Guid adminId)
    {
        if (Status != PayoutStatus.Pending)
            throw new BusinessException("Only pending payouts can be approved.", "PAYOUT_INVALID_STATUS");
        Status = PayoutStatus.Approved;
        ApprovedBy = adminId;
        ApprovedAt = DateTimeOffset.UtcNow;
        Version++;
    }

    public void MarkProcessing()
    {
        if (Status != PayoutStatus.Approved)
            throw new BusinessException("Only approved payouts can move to processing.", "PAYOUT_INVALID_STATUS");
        Status = PayoutStatus.Processing;
        Version++;
    }

    public void MarkCompleted()
    {
        if (Status != PayoutStatus.Processing && Status != PayoutStatus.Approved)
            throw new BusinessException("Only approved or processing payouts can be completed.", "PAYOUT_INVALID_STATUS");
        Status = PayoutStatus.Completed;
        CompletedAt = DateTimeOffset.UtcNow;
        Version++;
    }

    public void MarkFailed()
    {
        if (Status is PayoutStatus.Completed or PayoutStatus.Cancelled)
            throw new BusinessException("Cannot fail a completed or cancelled payout.", "PAYOUT_INVALID_STATUS");
        Status = PayoutStatus.Failed;
        Version++;
    }

    public void Cancel()
    {
        if (Status is PayoutStatus.Completed or PayoutStatus.Processing)
            throw new BusinessException("Cannot cancel a payout already in progress or completed.", "PAYOUT_INVALID_STATUS");
        Status = PayoutStatus.Cancelled;
        Version++;
    }
}

/// <summary>
/// A single booking included in a Payout. Denormalized so payout records stay
/// stable even if upstream booking/property records change.
/// </summary>
public class PayoutItem
{
    public Guid Id { get; private set; }
    public Guid PayoutId { get; private set; }
    public Guid BookingId { get; private set; }
    public Guid PaymentId { get; private set; }
    public decimal BookingTotal { get; private set; }   // Total guest paid
    public decimal ServiceFee { get; private set; }     // Platform fee withheld
    public decimal HostEarning { get; private set; }    // What host gets (BookingTotal - ServiceFee)
    public DateOnly CheckIn { get; private set; }
    public DateOnly CheckOut { get; private set; }
    public string PropertyTitle { get; private set; } = default!;
    public string GuestName { get; private set; } = default!;

    private PayoutItem() { } // EF

    public PayoutItem(
        Guid bookingId,
        Guid paymentId,
        decimal bookingTotal,
        decimal serviceFee,
        DateOnly checkIn,
        DateOnly checkOut,
        string propertyTitle,
        string guestName)
    {
        Id = Guid.CreateVersion7();
        BookingId = bookingId;
        PaymentId = paymentId;
        BookingTotal = bookingTotal;
        ServiceFee = serviceFee;
        HostEarning = bookingTotal - serviceFee;
        CheckIn = checkIn;
        CheckOut = checkOut;
        PropertyTitle = propertyTitle ?? string.Empty;
        GuestName = guestName ?? string.Empty;
    }
}

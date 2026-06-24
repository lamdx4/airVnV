using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.BookingService.Domain;

// ============================================================
// Enums
// ============================================================

public enum BookingStatus { Pending, AwaitingApproval, Confirmed, Refunding, RefundFailed, Cancelled }

// ============================================================
// Aggregate Root
// ============================================================

public class Booking : AggregateRoot
{
    public Guid Id { get; private set; }
    public Guid PropertyId { get; private set; }
    public Guid HostId { get; private set; }    
    public Guid GuestId { get; private set; }
    public string CountryCode { get; private set; } = default!;
    public string BookingMode { get; private set; } = default!;
    public DateOnly CheckIn { get; private set; }
    public DateOnly CheckOut { get; private set; }
    public int GuestCount { get; private set; }
    public int NightCount { get; private set; }
    public decimal BasePricePerNight { get; private set; }
    public decimal CleaningFee { get; private set; }
    public decimal ServiceFee { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal TotalPrice { get; private set; }
    public string CurrencyCode { get; private set; } = default!;
    public BookingStatus Status { get; private set; }
    public Guid? CancelledBy { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    // Bug #7 + Audit: track lifecycle timestamps and reason
    public DateTimeOffset? ConfirmedAt { get; private set; }
    public DateTimeOffset? CancelledAt { get; private set; }
    public string? CancelReason { get; private set; }

    private Booking() { } // EF Core

    public static Booking Create(
        Guid propertyId, Guid hostId, Guid guestId, string countryCode,
        DateOnly checkIn, DateOnly checkOut, int guestCount,
        decimal basePricePerNight, decimal cleaningFee, decimal serviceFee, decimal taxAmount, decimal totalPrice, string currencyCode, string bookingMode)
    {
        if (propertyId == Guid.Empty) throw new BusinessException("PropertyId cannot be empty.", "BOOKING_PROPERTY_REQUIRED");
        if (hostId == Guid.Empty) throw new BusinessException("HostId cannot be empty.", "BOOKING_HOST_REQUIRED");
        if (guestId == Guid.Empty) throw new BusinessException("GuestId cannot be empty.", "BOOKING_GUEST_REQUIRED");
        if (string.IsNullOrWhiteSpace(countryCode)) throw new BusinessException("CountryCode cannot be empty.", "BOOKING_COUNTRY_CODE_REQUIRED");
        if (checkOut <= checkIn) throw new BusinessException("CheckOut must be after CheckIn.", "BOOKING_INVALID_DATES");
        if (guestCount <= 0) throw new BusinessException("GuestCount must be greater than 0.", "BOOKING_INVALID_GUEST_COUNT");
        if (string.IsNullOrWhiteSpace(currencyCode)) throw new BusinessException("CurrencyCode cannot be empty.", "BOOKING_CURRENCY_REQUIRED");

        var nightCount = checkOut.DayNumber - checkIn.DayNumber;

        var booking = new Booking
        {
            Id = Guid.CreateVersion7(),
            PropertyId = propertyId,
            HostId = hostId,
            GuestId = guestId,
            CountryCode = countryCode.ToUpperInvariant(),
            CheckIn = checkIn,
            CheckOut = checkOut,
            GuestCount = guestCount,
            NightCount = nightCount,
            BasePricePerNight = basePricePerNight,
            CleaningFee = cleaningFee,
            ServiceFee = serviceFee,
            TaxAmount = taxAmount,
            TotalPrice = totalPrice,
            CurrencyCode = currencyCode,
            BookingMode = bookingMode,
            Status = BookingStatus.Pending,
            CreatedAt = DateTimeOffset.UtcNow,
        };

        booking.Raise(new BookingCreatedDomainEvent(
            booking.Id, booking.PropertyId, booking.GuestId, 
            booking.TotalPrice, booking.CurrencyCode, booking.CountryCode, booking.BookingMode,
            booking.Version));

        return booking;
    }

    /// <summary>
    /// Host approves: for RequestToBook, transitions to AwaitingApproval
    /// so the Saga can trigger the payment flow.
    /// </summary>
    public void Approve(Guid currentHostId)
    {
        if (currentHostId != HostId)
            throw new BusinessException("Only the host of this property can approve the booking.", "BOOKING_NOT_AUTHORIZED_HOST");
        AwaitForApproval();
    }

    public void Confirm()
    {
        if (Status != BookingStatus.Pending && Status != BookingStatus.AwaitingApproval)
            throw new BusinessException("Only Pending or AwaitingApproval bookings can be confirmed.", "BOOKING_INVALID_STATUS_FOR_CONFIRM");
            
        Status = BookingStatus.Confirmed;
        ConfirmedAt = DateTimeOffset.UtcNow;
        Version++;
        Raise(new BookingConfirmedDomainEvent(
            Id, 
            PropertyId, 
            GuestId, 
            TotalPrice, 
            CheckIn, 
            CheckOut, 
            Version));
    }

    // Bug #2 Fix: now raises BookingAwaitingApprovalDomainEvent so Saga can correlate
    public void AwaitForApproval()
    {
        if (Status != BookingStatus.Pending)
            throw new BusinessException("Only Pending bookings can transition to AwaitingApproval.", "BOOKING_INVALID_STATUS_FOR_AWAIT");
            
        Status = BookingStatus.AwaitingApproval;
        Version++;
        Raise(new BookingAwaitingApprovalDomainEvent(Id, PropertyId, HostId, GuestId, Version));
    }

    // Bug #7 Fix: Reject now passes PropertyId and Reason into the domain event
    public void Reject(Guid currentHostId)
    {
        if (currentHostId != HostId)
            throw new BusinessException("Only the host of this property can reject the booking.", "BOOKING_NOT_AUTHORIZED_HOST");
        if (Status != BookingStatus.Pending && Status != BookingStatus.AwaitingApproval)
            throw new BusinessException("Only Pending or AwaitingApproval bookings can be rejected.", "BOOKING_INVALID_STATUS_FOR_REJECT");
            
        Status = BookingStatus.Cancelled;
        CancelledBy = currentHostId;
        CancelledAt = DateTimeOffset.UtcNow;
        CancelReason = "Rejected by host";
        Version++;
        Raise(new BookingCancelledDomainEvent(Id, PropertyId, "Rejected by host", Version));
    }

    // Bug #7 Fix: Cancel now passes PropertyId and Reason into the domain event
    public void Cancel(Guid cancelledBy)
    {
        if (cancelledBy != GuestId && cancelledBy != HostId)
            throw new BusinessException("Unauthorized cancellation.", "BOOKING_UNAUTHORIZED_CANCEL");
        if (Status == BookingStatus.Cancelled)
             throw new BusinessException("Booking is already cancelled.", "BOOKING_ALREADY_CANCELLED");

        var reason = cancelledBy == GuestId ? "Cancelled by guest" : "Cancelled by host";

        Status = BookingStatus.Cancelled;
        CancelledBy = cancelledBy;
        CancelledAt = DateTimeOffset.UtcNow;
        CancelReason = reason;
        Version++;
        Raise(new BookingCancelledDomainEvent(Id, PropertyId, reason, Version));
    }

    /// <summary>
    /// Called when a CONFIRMED booking is being cancelled and a refund must be processed first.
    /// Transitions to Refunding state. The Saga will orchestrate the refund and then
    /// transition to Cancelled once the refund is confirmed by PaymentService.
    /// </summary>
    public void CancelAndRequestRefund(Guid cancelledBy)
    {
        if (cancelledBy != GuestId && cancelledBy != HostId)
            throw new BusinessException("Unauthorized cancellation.", "BOOKING_UNAUTHORIZED_CANCEL");
        if (Status != BookingStatus.Confirmed)
            throw new BusinessException("Only confirmed bookings can be refunded via this flow.", "BOOKING_INVALID_STATUS_FOR_REFUND");

        var reason = cancelledBy == GuestId ? "Cancelled by guest" : "Cancelled by host";

        Status = BookingStatus.Refunding;
        CancelledBy = cancelledBy;
        CancelledAt = DateTimeOffset.UtcNow;
        CancelReason = reason;
        Version++;
        Raise(new BookingRefundingDomainEvent(Id, PropertyId, reason, Version));
    }

    /// <summary>
    /// Called by the Saga (via PaymentRefundedConsumer) when the refund is confirmed.
    /// This is the terminal transition into Cancelled from Refunding.
    /// </summary>
    public void CompleteRefundCancellation()
    {
        if (Status != BookingStatus.Refunding)
            throw new BusinessException("Booking is not in Refunding state.", "BOOKING_INVALID_STATUS_FOR_COMPLETE_REFUND");

        Status = BookingStatus.Cancelled;
        Version++;
        Raise(new BookingCancelledDomainEvent(Id, PropertyId, CancelReason ?? "Refund completed", Version));
    }

    /// <summary>
    /// Called by the Saga when the refund attempt permanently failed (non-retryable).
    /// Booking stays locked. Admin intervention is required.
    /// </summary>
    public void MarkRefundFailed()
    {
        if (Status != BookingStatus.Refunding)
            throw new BusinessException("Booking is not in Refunding state.", "BOOKING_INVALID_STATUS_FOR_REFUND_FAILED");

        Status = BookingStatus.RefundFailed;
        Version++;
    }

    /// <summary>
    /// Called by external Consumers (e.g. BookingCancelledConsumer) to sync Domain state
    /// from an authoritative external signal (e.g. Saga). Does NOT raise a domain event
    /// to prevent an infinite publish → consume loop.
    /// </summary>
    public void SyncCancelled(string reason)
    {
        if (Status == BookingStatus.Cancelled) return; // Idempotent
        Status = BookingStatus.Cancelled;
        CancelledAt = DateTimeOffset.UtcNow;
        CancelReason = reason;
        Version++;
        // Intentionally NO Raise() here — the event already came from outside.
    }
}

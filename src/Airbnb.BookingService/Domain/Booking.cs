using System.ComponentModel.DataAnnotations;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.BookingService.Domain;

// ============================================================
// Enums
// ============================================================

public enum BookingStatus { Pending, AwaitingApproval, Confirmed, Cancelled }

// ============================================================
// Aggregate Root
// ============================================================

public class Booking : AggregateRoot
{
    public Guid Id { get; private set; }
    public Guid PropertyId { get; private set; }
    public Guid HostId { get; private set; }    
    public Guid GuestId { get; private set; }
    public string CountryCode { get; private set; } = default!; // Added for payment routing
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

    public void Approve(Guid currentHostId)
    {
        if (currentHostId != HostId)
            throw new BusinessException("Only the host of this property can approve the booking.", "BOOKING_NOT_AUTHORIZED_HOST");
        Confirm();
    }

    public void Confirm()
    {
        if (Status != BookingStatus.Pending && Status != BookingStatus.AwaitingApproval)
            throw new BusinessException("Only Pending or AwaitingApproval bookings can be confirmed.", "BOOKING_INVALID_STATUS_FOR_CONFIRM");
            
        Status = BookingStatus.Confirmed;
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

    public void AwaitForApproval()
    {
        if (Status != BookingStatus.Pending)
            throw new BusinessException("Only Pending bookings can transition to AwaitingApproval.", "BOOKING_INVALID_STATUS_FOR_AWAIT");
            
        Status = BookingStatus.AwaitingApproval;
        Version++;
        // Ghi chú: Không có Domain Event mới ở đây vì Saga State Machine sẽ ghi nhận trạng thái.
        // Có thể bổ sung event sau nếu các services khác cần theo dõi.
    }

    public void Reject(Guid currentHostId)
    {
        if (currentHostId != HostId)
            throw new BusinessException("Only the host of this property can reject the booking.", "BOOKING_NOT_AUTHORIZED_HOST");
        if (Status != BookingStatus.Pending && Status != BookingStatus.AwaitingApproval)
            throw new BusinessException("Only Pending or AwaitingApproval bookings can be rejected.", "BOOKING_INVALID_STATUS_FOR_REJECT");
            
        Status = BookingStatus.Cancelled;
        CancelledBy = currentHostId;
        Version++;
        Raise(new BookingCancelledDomainEvent(Id, Version));
    }

    public void Cancel(Guid cancelledBy)
    {
        if (cancelledBy != GuestId && cancelledBy != HostId)
            throw new BusinessException("Unauthorized cancellation.", "BOOKING_UNAUTHORIZED_CANCEL");
        if (Status == BookingStatus.Cancelled)
             throw new BusinessException("Booking is already cancelled.", "BOOKING_ALREADY_CANCELLED");
             
        Status = BookingStatus.Cancelled;
        CancelledBy = cancelledBy;
        Version++;
        Raise(new BookingCancelledDomainEvent(Id, Version));
    }
}

public class ProcessedEvent
{
    public Guid EventId { get; set; }
    public string EventType { get; set; } = default!;
    public DateTimeOffset ProcessedAt { get; set; }
}

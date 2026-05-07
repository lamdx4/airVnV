using System.ComponentModel.DataAnnotations;

namespace Airbnb.BookingService.Domain;

// ============================================================
// Enums
// ============================================================

public enum BookingStatus { Pending, Confirmed, Cancelled }

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
        decimal basePricePerNight, decimal cleaningFee, decimal serviceFee, decimal taxAmount, decimal totalPrice, string currencyCode)
    {
        if (propertyId == Guid.Empty) throw new ArgumentException("PropertyId cannot be empty.");
        if (hostId == Guid.Empty) throw new ArgumentException("HostId cannot be empty.");
        if (guestId == Guid.Empty) throw new ArgumentException("GuestId cannot be empty.");
        if (string.IsNullOrWhiteSpace(countryCode)) throw new ArgumentException("CountryCode cannot be empty.");
        if (checkOut <= checkIn) throw new ArgumentException("CheckOut must be after CheckIn.");
        if (guestCount <= 0) throw new ArgumentException("GuestCount must be greater than 0.");
        if (string.IsNullOrWhiteSpace(currencyCode)) throw new ArgumentException("CurrencyCode cannot be empty.");

        var nightCount = checkOut.DayNumber - checkIn.DayNumber;

        return new Booking
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
            Status = BookingStatus.Pending,
            CreatedAt = DateTimeOffset.UtcNow,
        };
    }

    public void Approve(Guid currentHostId)
    {
        if (currentHostId != HostId)
            throw new InvalidOperationException("Only the host of this property can approve the booking.");
        Confirm();
    }

    public void Confirm()
    {
        if (Status != BookingStatus.Pending)
            throw new InvalidOperationException("Only Pending bookings can be confirmed.");
            
        Status = BookingStatus.Confirmed;
    }

    public void Reject(Guid currentHostId)
    {
        if (currentHostId != HostId)
            throw new InvalidOperationException("Only the host of this property can reject the booking.");
        if (Status != BookingStatus.Pending)
            throw new InvalidOperationException("Only Pending bookings can be rejected.");
            
        Status = BookingStatus.Cancelled;
        CancelledBy = currentHostId;
    }

    public void Cancel(Guid cancelledBy)
    {
        if (cancelledBy != GuestId && cancelledBy != HostId)
            throw new InvalidOperationException("Unauthorized cancellation.");
        if (Status == BookingStatus.Cancelled)
             throw new InvalidOperationException("Booking is already cancelled.");
             
        Status = BookingStatus.Cancelled;
        CancelledBy = cancelledBy;
    }
}

public class ProcessedEvent
{
    public Guid EventId { get; set; }
    public string EventType { get; set; } = default!;
    public DateTimeOffset ProcessedAt { get; set; }
}

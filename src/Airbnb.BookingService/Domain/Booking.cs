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
    public Guid UserId { get; private set; }
    public DateTimeOffset CheckIn { get; private set; }
    public DateTimeOffset CheckOut { get; private set; }
    public decimal TotalPrice { get; private set; }
    public BookingStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private Booking() { } // EF Core

    public static Booking Create(Guid propertyId, Guid userId, DateTimeOffset checkIn, DateTimeOffset checkOut, decimal totalPrice)
    {
        if (propertyId == Guid.Empty) throw new ArgumentException("PropertyId cannot be empty.");
        if (userId == Guid.Empty) throw new ArgumentException("UserId cannot be empty.");
        if (checkOut <= checkIn) throw new ArgumentException("CheckOut must be after CheckIn.");
        if (totalPrice <= 0) throw new ArgumentException("TotalPrice must be greater than 0.");

        return new Booking
        {
            Id = Guid.NewGuid(),
            PropertyId = propertyId,
            UserId = userId,
            CheckIn = checkIn,
            CheckOut = checkOut,
            TotalPrice = totalPrice,
            Status = BookingStatus.Pending,
            CreatedAt = DateTimeOffset.UtcNow,
        };
    }

    public void Confirm()
    {
        if (Status != BookingStatus.Pending)
            throw new InvalidOperationException("Only Pending bookings can be confirmed.");
        Status = BookingStatus.Confirmed;
        // TODO: Raise(new BookingConfirmedEvent(Id, PropertyId, UserId));
    }

    public void Cancel()
    {
        if (Status == BookingStatus.Cancelled)
            throw new InvalidOperationException("Booking is already cancelled.");
        Status = BookingStatus.Cancelled;
        // TODO: Raise(new BookingCancelledEvent(Id, PropertyId, UserId));
    }
}

// ============================================================
// Idempotency – Consumer side (unique constraint on EventId)
// ============================================================

public class ProcessedEvent
{
    [Key]
    public Guid EventId { get; set; }
    public DateTimeOffset ProcessedAt { get; set; } = DateTimeOffset.UtcNow;
    public string EventType { get; set; } = default!;
}

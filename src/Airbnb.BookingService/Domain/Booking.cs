using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Airbnb.BookingService.Domain;

public class Booking(Guid propertyId, Guid userId, DateTime checkIn, DateTime checkOut, decimal totalPrice)
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid PropertyId { get; private set; } = propertyId;
    public Guid UserId { get; private set; } = userId;
    public DateTime CheckIn { get; private set; } = checkIn;
    public DateTime CheckOut { get; private set; } = checkOut;
    public decimal TotalPrice { get; private set; } = totalPrice;
    public BookingStatus Status { get; private set; } = BookingStatus.Pending;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public void Confirm() => Status = BookingStatus.Confirmed;
}

public enum BookingStatus { Pending, Confirmed, Cancelled }

// Staff-level: Idempotency table
public class ProcessedEvent
{
    [Key]
    public Guid EventId { get; set; }
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
    public string EventType { get; set; } = default!;
}

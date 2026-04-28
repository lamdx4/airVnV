using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Airbnb.PaymentService.Domain;

public class Payment(Guid bookingId, decimal amount)
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid BookingId { get; private set; } = bookingId;
    public decimal Amount { get; private set; } = amount;
    public PaymentStatus Status { get; private set; } = PaymentStatus.Pending;
    public string? TransactionId { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public void MarkAsSuccess(string transactionId)
    {
        Status = PaymentStatus.Success;
        TransactionId = transactionId;
    }
}

public enum PaymentStatus { Pending, Success, Failed }

// Staff-level: Outbox Entity
public class OutboxEvent
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public string EventType { get; set; } = default!;
    public string Payload { get; set; } = default!;
    public string? TraceId { get; set; } // Lưu TraceId để Debezium SMT có thể lấy
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool Processed { get; set; } = false; // Có thể dùng cho Polling Publisher nếu không dùng CDC
}

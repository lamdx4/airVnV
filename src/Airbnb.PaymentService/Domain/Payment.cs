using System.ComponentModel.DataAnnotations;

namespace Airbnb.PaymentService.Domain;

// ============================================================
// Enums
// ============================================================

public enum PaymentStatus { Pending, Success, Failed }

// ============================================================
// Aggregate Root
// ============================================================

public class Payment : AggregateRoot
{
    public Guid Id { get; private set; }
    public Guid BookingId { get; private set; }
    public decimal Amount { get; private set; }
    public PaymentStatus Status { get; private set; }
    public string? TransactionId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private Payment() { } // EF Core

    public static Payment Create(Guid bookingId, decimal amount)
    {
        if (bookingId == Guid.Empty) throw new ArgumentException("BookingId cannot be empty.");
        if (amount <= 0) throw new ArgumentException("Amount must be greater than 0.");

        return new Payment
        {
            Id = Guid.NewGuid(),
            BookingId = bookingId,
            Amount = amount,
            Status = PaymentStatus.Pending,
            CreatedAt = DateTimeOffset.UtcNow,
        };
    }

    public void MarkAsSuccess(string transactionId)
    {
        if (string.IsNullOrWhiteSpace(transactionId))
            throw new ArgumentException("TransactionId is required.");
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException("Only Pending payments can be marked as success.");
        Status = PaymentStatus.Success;
        TransactionId = transactionId;
        // TODO: Raise(new PaymentSucceededEvent(Id, BookingId, Amount));
    }

    public void MarkAsFailed()
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException("Only Pending payments can be marked as failed.");
        Status = PaymentStatus.Failed;
        // TODO: Raise(new PaymentFailedEvent(Id, BookingId));
    }
}

// ============================================================
// Outbox Entity (CAP-style – giữ nếu dùng Debezium CDC)
// ============================================================

public class OutboxEvent
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public string EventType { get; set; } = default!;
    public string Payload { get; set; } = default!;
    public string? TraceId { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public bool Processed { get; set; } = false;
}

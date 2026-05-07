using System.ComponentModel.DataAnnotations;
using Airbnb.SharedKernel;

namespace Airbnb.PaymentService.Domain;

public enum PaymentStatus { Pending, Success, Failed, Expired }

public class Payment : AggregateRoot
{
    public Guid Id { get; private set; }
    public Guid BookingId { get; private set; }
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = default!;
    public PaymentStatus Status { get; private set; }
    public string? TransactionId { get; private set; } // External ID from Provider
    public string? PaymentUrl { get; private set; }
    public DateTimeOffset? ExpiresAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private Payment() { } // EF Core

    public static Payment Create(Guid bookingId, decimal amount, string currency)
    {
        if (bookingId == Guid.Empty) throw new ArgumentException("BookingId cannot be empty.");
        if (amount <= 0) throw new ArgumentException("Amount must be greater than 0.");
        if (string.IsNullOrWhiteSpace(currency)) throw new ArgumentException("Currency is required.");

        var payment = new Payment
        {
            Id = Guid.CreateVersion7(),
            BookingId = bookingId,
            Amount = amount,
            Currency = currency.ToUpperInvariant(),
            Status = PaymentStatus.Pending,
            CreatedAt = DateTimeOffset.UtcNow,
        };

        payment.Raise(new PaymentInitiatedDomainEvent(
            payment.Id, payment.BookingId, payment.Amount, payment.Currency, payment.Version));

        return payment;
    }

    public void Initiate(string paymentUrl, DateTimeOffset expiresAt)
    {
        if (string.IsNullOrWhiteSpace(paymentUrl)) throw new ArgumentException("PaymentUrl is required.");
        if (expiresAt <= DateTimeOffset.UtcNow) throw new ArgumentException("ExpiresAt must be in the future.");

        PaymentUrl = paymentUrl;
        ExpiresAt = expiresAt;
    }

    public void MarkAsSuccess(string transactionId)
    {
        if (string.IsNullOrWhiteSpace(transactionId))
            throw new ArgumentException("TransactionId is required.");
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException("Only Pending payments can be marked as success.");
        
        Status = PaymentStatus.Success;
        TransactionId = transactionId;
        Version++;
        Raise(new PaymentSucceededDomainEvent(Id, BookingId, Amount, Currency, TransactionId, Version));
    }

    public void MarkAsFailed()
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException("Only Pending payments can be marked as failed.");
        Status = PaymentStatus.Failed;
        Version++;
        Raise(new PaymentFailedDomainEvent(Id, BookingId, "PAYMENT_FAILED", Version));
    }

    public void MarkAsExpired()
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException("Only Pending payments can be marked as expired.");
        Status = PaymentStatus.Expired;
    }

    public bool IsStillValid() 
        => Status == PaymentStatus.Pending && ExpiresAt > DateTimeOffset.UtcNow.AddMinutes(2);
}

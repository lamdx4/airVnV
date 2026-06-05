using System.ComponentModel.DataAnnotations;
using Airbnb.SharedKernel;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Domain;

public enum PaymentStatus { Pending, Success, Failed, Expired, Refunded, PartiallyRefunded }

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
        if (bookingId == Guid.Empty) throw new BusinessException("BookingId cannot be empty.", "PAYMENT_BOOKING_REQUIRED");
        if (amount <= 0) throw new BusinessException("Amount must be greater than 0.", "PAYMENT_INVALID_AMOUNT");
        if (string.IsNullOrWhiteSpace(currency)) throw new BusinessException("Currency is required.", "PAYMENT_CURRENCY_REQUIRED");

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
        if (string.IsNullOrWhiteSpace(paymentUrl)) throw new BusinessException("PaymentUrl is required.", "PAYMENT_URL_REQUIRED");
        if (expiresAt <= DateTimeOffset.UtcNow) throw new BusinessException("ExpiresAt must be in the future.", "PAYMENT_INVALID_EXPIRATION");

        PaymentUrl = paymentUrl;
        ExpiresAt = expiresAt;
    }

    public void MarkAsSuccess(string transactionId)
    {
        if (string.IsNullOrWhiteSpace(transactionId))
            throw new BusinessException("TransactionId is required.", "PAYMENT_TRANSACTION_ID_REQUIRED");
        if (Status != PaymentStatus.Pending)
            throw new BusinessException("Only Pending payments can be marked as success.", "PAYMENT_INVALID_STATUS");
        
        Status = PaymentStatus.Success;
        TransactionId = transactionId;
        Version++;
        Raise(new PaymentSucceededDomainEvent(Id, BookingId, Amount, Currency, TransactionId, Version));
    }

    public void MarkAsFailed()
    {
        if (Status != PaymentStatus.Pending)
            throw new BusinessException("Only Pending payments can be marked as failed.", "PAYMENT_INVALID_STATUS");
        Status = PaymentStatus.Failed;
        Version++;
        Raise(new PaymentFailedDomainEvent(Id, BookingId, "PAYMENT_FAILED", Version));
    }

    public void MarkAsExpired()
    {
        if (Status != PaymentStatus.Pending)
            throw new BusinessException("Only Pending payments can be marked as expired.", "PAYMENT_INVALID_STATUS");
        Status = PaymentStatus.Expired;
    }

    public bool IsStillValid() 
        => Status == PaymentStatus.Pending && ExpiresAt > DateTimeOffset.UtcNow.AddMinutes(2);

    public void MarkAsRefunded()
    {
        if (Status != PaymentStatus.Success && Status != PaymentStatus.PartiallyRefunded)
            throw new InvalidOperationException("Only Success or PartiallyRefunded payments can be fully refunded.");
        Status = PaymentStatus.Refunded;
        Version++;
    }

    public void MarkAsPartiallyRefunded()
    {
        if (Status != PaymentStatus.Success)
            throw new InvalidOperationException("Only Success payments can be partially refunded.");
        Status = PaymentStatus.PartiallyRefunded;
        Version++;
    }
}

using Airbnb.SharedKernel.Domain;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Domain;

/// <summary>
/// Immutable audit record for every refund (full or partial) issued against a Payment.
/// </summary>
public class RefundRecord
{
    public Guid Id { get; private set; }
    public Guid PaymentId { get; private set; }
    public decimal Amount { get; private set; }
    public string Reason { get; private set; } = default!;
    public bool IsFullRefund { get; private set; }
    public Guid PerformedBy { get; private set; }
    public Guid? TicketId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private RefundRecord() { } // EF

    public static RefundRecord Create(
        Guid paymentId,
        decimal amount,
        string reason,
        bool isFullRefund,
        Guid performedBy,
        Guid? ticketId = null)
    {
        if (paymentId == Guid.Empty)
            throw new BusinessException("PaymentId is required.", "REFUND_PAYMENT_REQUIRED");
        if (amount <= 0)
            throw new BusinessException("Refund amount must be greater than 0.", "REFUND_INVALID_AMOUNT");
        if (string.IsNullOrWhiteSpace(reason))
            throw new BusinessException("Refund reason is required.", "REFUND_REASON_REQUIRED");

        return new RefundRecord
        {
            Id = Guid.CreateVersion7(),
            PaymentId = paymentId,
            Amount = amount,
            Reason = reason.Trim(),
            IsFullRefund = isFullRefund,
            PerformedBy = performedBy,
            TicketId = ticketId,
            CreatedAt = DateTimeOffset.UtcNow,
        };
    }
}

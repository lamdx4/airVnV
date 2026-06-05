namespace Airbnb.PaymentService.Domain;

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

    // Navigation
    public Payment Payment { get; private set; } = default!;

    private RefundRecord() { } // EF Core

    public static RefundRecord Create(Guid paymentId, decimal amount, string reason, bool isFullRefund, Guid performedBy, Guid? ticketId = null)
    {
        if (paymentId == Guid.Empty) throw new ArgumentException("PaymentId cannot be empty.");
        if (amount <= 0) throw new ArgumentException("Refund amount must be greater than 0.");
        if (string.IsNullOrWhiteSpace(reason)) throw new ArgumentException("Refund reason is required.");
        if (performedBy == Guid.Empty) throw new ArgumentException("PerformedBy cannot be empty.");

        return new RefundRecord
        {
            Id = Guid.CreateVersion7(),
            PaymentId = paymentId,
            Amount = amount,
            Reason = reason,
            IsFullRefund = isFullRefund,
            PerformedBy = performedBy,
            TicketId = ticketId,
            CreatedAt = DateTimeOffset.UtcNow,
        };
    }
}

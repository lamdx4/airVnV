namespace Airbnb.ChatService.Domain.Tickets;

public enum RefundStatus { Pending, Processing, Completed, Failed, Cancelled }
public enum RefundType { Full, Partial, Cancellation, Compensation }

public class RefundRequest
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    
    // Context
    public Guid BookingId { get; set; }
    public Guid GuestId { get; set; }
    public Guid HostId { get; set; }
    
    // Refund Details
    public RefundType Type { get; set; }
    public decimal TotalAmount { get; set; }           // Original booking amount
    public decimal RequestedAmount { get; set; }      // Amount to refund
    public string Currency { get; set; } = "VND";
    
    // Reason
    public string Reason { get; set; } = default!;
    public TicketCategory Category { get; set; }
    public string? EvidenceUrls { get; set; }         // JSON array of attachment URLs
    
    // Status
    public RefundStatus Status { get; set; } = RefundStatus.Pending;
    
    // Processing info
    public string? TransactionId { get; set; }        // External payment tx ID
    public Guid? ProcessedById { get; set; }          // Admin who processed
    public string? ProcessedByName { get; set; }
    public DateTimeOffset? ProcessedAt { get; set; }
    
    // Linked Ticket
    public Guid? RelatedTicketId { get; set; }
    
    // Timestamps
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}

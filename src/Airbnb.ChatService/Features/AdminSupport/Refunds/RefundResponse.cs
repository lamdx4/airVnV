namespace Airbnb.ChatService.Features.AdminSupport.Refunds;

public class RefundResponse
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public Guid GuestId { get; set; }
    public Guid HostId { get; set; }
    public string Type { get; set; } = default!;
    public decimal TotalAmount { get; set; }
    public decimal RequestedAmount { get; set; }
    public string Currency { get; set; } = "VND";
    public string Reason { get; set; } = default!;
    public string Category { get; set; } = default!;
    public string Status { get; set; } = default!;
    public string? TransactionId { get; set; }
    public Guid? ProcessedById { get; set; }
    public string? ProcessedByName { get; set; }
    public DateTimeOffset? ProcessedAt { get; set; }
    public Guid? RelatedTicketId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class RefundListResponse
{
    public List<RefundResponse> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public RefundStats Stats { get; set; } = new();
}

public class RefundStats
{
    public decimal TotalPendingAmount { get; set; }
    public int TotalPending { get; set; }
    public int TotalProcessing { get; set; }
    public int TotalCompleted { get; set; }
    public int TotalFailed { get; set; }
}

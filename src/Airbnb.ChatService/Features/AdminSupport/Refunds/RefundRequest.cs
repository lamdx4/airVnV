namespace Airbnb.ChatService.Features.AdminSupport.Refunds;

public class GetRefundsRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Status { get; set; }
    public string? Type { get; set; }
    public string? GuestId { get; set; }
    public string? HostId { get; set; }
    public string? BookingId { get; set; }
    public DateTimeOffset? FromDate { get; set; }
    public DateTimeOffset? ToDate { get; set; }
    public string SortBy { get; set; } = "CreatedAt";
    public string SortOrder { get; set; } = "desc";
}

public class ProcessRefundRequest
{
    public Guid RefundId { get; set; }
    public string Action { get; set; } = default!; // "approve" | "reject" | "process"
    public string? RejectionReason { get; set; }
}

public class CreateRefundRequest
{
    public Guid BookingId { get; set; }
    public Guid GuestId { get; set; }
    public Guid HostId { get; set; }
    public string Type { get; set; } = default!;
    public decimal RequestedAmount { get; set; }
    public string Reason { get; set; } = default!;
    public string Category { get; set; } = default!;
    public Guid? RelatedTicketId { get; set; }
}

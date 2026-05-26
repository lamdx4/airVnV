namespace Airbnb.PaymentService.Features.TransactionHistory;

public class Request
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? TransactionId { get; set; }
    public string? Status { get; set; }
    public string? GuestId { get; set; }
    public string? HostId { get; set; }
    public string? Currency { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
    public DateTimeOffset? FromDate { get; set; }
    public DateTimeOffset? ToDate { get; set; }
    public string SortBy { get; set; } = "CreatedAt";
    public string SortOrder { get; set; } = "desc";
}

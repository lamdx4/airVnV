namespace Airbnb.PaymentService.Features.TransactionHistory;

public class Response
{
    public List<TransactionItem> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public TransactionSummary Summary { get; set; } = new();
}

public class TransactionItem
{
    public Guid PaymentId { get; set; }
    public Guid BookingId { get; set; }
    public string? TransactionId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "VND";
    public string Status { get; set; } = default!;
    public string Type { get; set; } = "PayIn"; // PayIn | PayOut
    public decimal PlatformFee { get; set; }
    public decimal NetAmount { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? ProcessedAt { get; set; }
}

public class TransactionSummary
{
    public decimal TotalPayIn { get; set; }
    public decimal TotalPayOut { get; set; }
    public int TotalTransactions { get; set; }
    public int SuccessTransactions { get; set; }
    public int FailedTransactions { get; set; }
    public int PendingTransactions { get; set; }
}

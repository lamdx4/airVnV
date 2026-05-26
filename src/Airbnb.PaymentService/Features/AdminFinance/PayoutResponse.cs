namespace Airbnb.PaymentService.Features.AdminFinance;

public class PayoutResponse
{
    public List<PayoutItem> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

public class PayoutItem
{
    public Guid PayoutId { get; set; }
    public Guid HostId { get; set; }
    public string HostName { get; set; } = default!;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "VND";
    public string Status { get; set; } = "Pending";
    public int BookingCount { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? ProcessedAt { get; set; }
}

public class DashboardFinanceResponse
{
    public decimal TotalPayIn { get; set; }
    public decimal TotalPayOut { get; set; }
    public decimal PlatformRevenue { get; set; }
    public decimal PendingPayouts { get; set; }
    public int PendingPayoutCount { get; set; }
    public decimal AverageTransactionAmount { get; set; }
    public List<DailyFinanceStat> DailyStats { get; set; } = new();
}

public class DailyFinanceStat
{
    public string Date { get; set; } = default!;
    public decimal PayIn { get; set; }
    public decimal PayOut { get; set; }
    public decimal Revenue { get; set; }
}

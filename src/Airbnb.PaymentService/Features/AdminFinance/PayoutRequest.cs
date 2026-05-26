namespace Airbnb.PaymentService.Features.AdminFinance;

public class PayoutRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? HostId { get; set; }
    public string? Status { get; set; }
    public DateTimeOffset? FromDate { get; set; }
    public DateTimeOffset? ToDate { get; set; }
}

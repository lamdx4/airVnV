namespace Airbnb.PaymentService.Features.AdminFinance;

public class DashboardRequest
{
    public DateTimeOffset? FromDate { get; set; }
    public DateTimeOffset? ToDate { get; set; }
}

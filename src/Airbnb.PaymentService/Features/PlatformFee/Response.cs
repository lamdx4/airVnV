namespace Airbnb.PaymentService.Features.PlatformFee;

public class Response
{
    public decimal HostFeePercent { get; set; } = 10.0m;
    public decimal GuestFeePercent { get; set; } = 0.0m;
    public decimal DefaultPlatformFeePercent { get; set; } = 10.0m;
    public string LastUpdatedBy { get; set; } = "system";
    public DateTimeOffset LastUpdatedAt { get; set; }
}

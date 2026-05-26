namespace Airbnb.PaymentService.Features.PlatformFee;

public class Request
{
    public decimal? HostFeePercent { get; set; }
    public decimal? GuestFeePercent { get; set; }
}

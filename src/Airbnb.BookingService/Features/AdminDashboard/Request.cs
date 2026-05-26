namespace Airbnb.BookingService.Features.AdminDashboard;

public class Request
{
    public DateTimeOffset? FromDate { get; set; }
    public DateTimeOffset? ToDate { get; set; }
}

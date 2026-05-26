namespace Airbnb.BookingService.Features.AdminDashboard;

public class Response
{
    public int TotalBookings { get; set; }
    public int NewBookings { get; set; }
    public int ConfirmedBookings { get; set; }
    public int CancelledBookings { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal GmvVnd { get; set; }
    public int TotalProperties { get; set; }
    public decimal AverageOccupancyRate { get; set; }
    public int TotalGuests { get; set; }
    public int TotalHosts { get; set; }
    public PeriodStats DailyStats { get; set; } = new();
}

public class PeriodStats
{
    public List<DailyStatItem> Items { get; set; } = new();
}

public class DailyStatItem
{
    public string Date { get; set; } = default!;
    public int BookingCount { get; set; }
    public decimal Revenue { get; set; }
    public int NewUsers { get; set; }
}

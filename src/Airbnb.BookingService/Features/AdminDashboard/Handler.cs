using Airbnb.BookingService.Domain;
using Airbnb.BookingService.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.BookingService.Features.AdminDashboard;

public class Handler(BookingDbContext dbContext)
{
    public async Task<Response> Handle(Request request, CancellationToken ct)
    {
        var fromDate = request.FromDate ?? DateTimeOffset.UtcNow.AddDays(-30);
        var toDate = request.ToDate ?? DateTimeOffset.UtcNow;

        // Get booking statistics
        var allBookings = dbContext.Bookings.AsNoTracking();
        var periodBookings = allBookings.Where(b => b.CreatedAt >= fromDate && b.CreatedAt <= toDate);

        var totalBookings = await allBookings.CountAsync(ct);
        var newBookings = await periodBookings.CountAsync(ct);
        var confirmedBookings = await periodBookings.CountAsync(b => b.Status == BookingStatus.Confirmed, ct);
        var cancelledBookings = await periodBookings.CountAsync(b => b.Status == BookingStatus.Cancelled, ct);
        var totalRevenue = await dbContext.Bookings
            .Where(b => b.Status == BookingStatus.Confirmed)
            .SumAsync(b => b.TotalPrice, ct);

        // Calculate daily stats for the chart
        var dailyStats = await periodBookings
            .Where(b => b.Status == BookingStatus.Confirmed)
            .GroupBy(b => b.CreatedAt.Date)
            .Select(g => new DailyStatItem
            {
                Date = g.Key.ToString("yyyy-MM-dd"),
                BookingCount = g.Count(),
                Revenue = g.Sum(b => b.TotalPrice),
                NewUsers = 0 // Will be filled from UserService if needed
            })
            .OrderBy(x => x.Date)
            .Take(30)
            .ToListAsync(ct);

        // Placeholder values - these would ideally come from UserService or PropertyService
        var totalProperties = 0;
        var averageOccupancyRate = totalBookings > 0 ? (decimal)confirmedBookings / totalBookings * 100 : 0;
        var totalGuests = await dbContext.Bookings.Select(b => b.GuestId).Distinct().CountAsync(ct);
        var totalHosts = await dbContext.Bookings.Select(b => b.HostId).Distinct().CountAsync(ct);

        return new Response
        {
            TotalBookings = totalBookings,
            NewBookings = newBookings,
            ConfirmedBookings = confirmedBookings,
            CancelledBookings = cancelledBookings,
            TotalRevenue = totalRevenue,
            GmvVnd = totalRevenue, // Assuming VND
            TotalProperties = totalProperties,
            AverageOccupancyRate = Math.Round(averageOccupancyRate, 2),
            TotalGuests = totalGuests,
            TotalHosts = totalHosts,
            DailyStats = new PeriodStats
            {
                Items = dailyStats
            }
        };
    }
}

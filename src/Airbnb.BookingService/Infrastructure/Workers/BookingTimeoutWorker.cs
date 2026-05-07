using Airbnb.BookingService.Domain;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.BookingService.Infrastructure.Workers;

public class BookingTimeoutWorker(
    IServiceScopeFactory scopeFactory,
    ILogger<BookingTimeoutWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("BookingTimeoutWorker started. Checking for expired pending bookings every 5 minutes.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<BookingDbContext>();

                // Expire bookings that have been pending for more than 24 hours
                var cutoffTime = DateTimeOffset.UtcNow.AddHours(-24);

                var expiredBookings = await db.Bookings
                    .Where(b => b.Status == BookingStatus.Pending && b.CreatedAt < cutoffTime)
                    .ToListAsync(stoppingToken);

                if (expiredBookings.Any())
                {
                    logger.LogInformation("Found {Count} expired pending bookings. Cancelling them...", expiredBookings.Count);

                    foreach (var booking in expiredBookings)
                    {
                        try
                        {
                            // Using the HostId as the one who implicitly "rejected" it due to timeout
                            // or using Guid.Empty to signify System
                            booking.Reject(booking.HostId);
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "Failed to auto-reject booking {BookingId}", booking.Id);
                        }
                    }

                    await db.SaveChangesAsync(stoppingToken);
                    logger.LogInformation("Successfully auto-rejected {Count} bookings.", expiredBookings.Count);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred executing BookingTimeoutWorker.");
            }

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}

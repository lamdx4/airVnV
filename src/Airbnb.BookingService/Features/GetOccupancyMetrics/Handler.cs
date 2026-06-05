using Microsoft.EntityFrameworkCore;
using Airbnb.BookingService.Infrastructure;
using Airbnb.BookingService.Domain;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.BookingService.Features.GetOccupancyMetrics;

public sealed class Handler(BookingDbContext db)
    : Mediator.IQueryHandler<Request, ApiResponse<OccupancyMetricsResponse>>
{
    public async ValueTask<ApiResponse<OccupancyMetricsResponse>> Handle(Request req, CancellationToken ct)
    {
        // Range days (inclusive)
        var rangeDays = Math.Max(0, req.To.DayNumber - req.From.DayNumber + 1);

        // Booked nights = sum of (CheckOut - CheckIn) for Confirmed bookings overlapping the range.
        // "Overlapping" defined as: booking CheckIn <= range.To AND booking CheckOut > range.From.
        // We materialize CheckIn/CheckOut as DateOnly and compare.
        var fromDay = req.From.DayNumber;
        var toDay = req.To.DayNumber;

        var rows = await db.Bookings.AsNoTracking()
            .Where(b => b.Status == BookingStatus.Confirmed)
            .Where(b => b.CheckIn.DayNumber <= toDay && b.CheckOut.DayNumber > fromDay)
            .Select(b => new { b.CheckIn, b.CheckOut })
            .ToListAsync(ct);

        // For each booking, clamp its nights to the range.
        long bookedNights = 0;
        foreach (var r in rows)
        {
            var effectiveStart = r.CheckIn.DayNumber < fromDay ? fromDay : r.CheckIn.DayNumber;
            var effectiveEnd = r.CheckOut.DayNumber - 1;
            effectiveEnd = effectiveEnd > toDay ? toDay : effectiveEnd;
            var nights = effectiveEnd - effectiveStart + 1;
            if (nights > 0) bookedNights += nights;
        }

        return ApiResponse<OccupancyMetricsResponse>.SuccessResult(
            new OccupancyMetricsResponse(bookedNights, rangeDays)
        );
    }
}

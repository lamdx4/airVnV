using Microsoft.EntityFrameworkCore;
using Airbnb.BookingService.Infrastructure;
using Airbnb.BookingService.Domain;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.BookingService.Features.GetBookingSummary;

public sealed class Handler(BookingDbContext db)
    : Mediator.IQueryHandler<Request, ApiResponse<BookingSummaryResponse>>
{
    public async ValueTask<ApiResponse<BookingSummaryResponse>> Handle(Request req, CancellationToken ct)
    {
        // Inclusive date range: bookings created on `from` or `to` are included.
        var fromStart = req.From.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var toEnd = req.To.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);

        var confirmedInRange = await db.Bookings.AsNoTracking()
            .Where(b => b.Status == BookingStatus.Confirmed)
            .Where(b => b.CreatedAt >= fromStart && b.CreatedAt <= toEnd)
            .Select(b => b.TotalPrice)
            .ToListAsync(ct);

        var totalBookings = confirmedInRange.Count;
        var totalRevenue = confirmedInRange.Sum();
        var averageBookingValue = totalBookings > 0 ? Math.Round(totalRevenue / totalBookings, 2) : 0m;

        return ApiResponse<BookingSummaryResponse>.SuccessResult(
            new BookingSummaryResponse(totalBookings, totalRevenue, averageBookingValue)
        );
    }
}

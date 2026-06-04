using Microsoft.EntityFrameworkCore;
using Airbnb.BookingService.Infrastructure;
using Airbnb.BookingService.Domain;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.BookingService.Features.GetAdminStats;

public sealed class Handler(BookingDbContext db)
    : Mediator.IQueryHandler<Request, ApiResponse<BookingStatsResponse>>
{
    public async ValueTask<ApiResponse<BookingStatsResponse>> Handle(Request req, CancellationToken ct)
    {
        var totalBookings = await db.Bookings.AsNoTracking().CountAsync(ct);

        var confirmedCount = await db.Bookings.AsNoTracking()
            .Where(b => b.Status == BookingStatus.Confirmed)
            .ToListAsync(ct);

        var activeBookings = await db.Bookings.AsNoTracking()
            .CountAsync(b => b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.AwaitingApproval, ct);

        var totalRevenue = confirmedCount.Sum(b => b.TotalPrice);

        return ApiResponse<BookingStatsResponse>.SuccessResult(
            new BookingStatsResponse(totalBookings, activeBookings, totalRevenue)
        );
    }
}

using Airbnb.BookingService.Domain;
using Airbnb.BookingService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.BookingService.Features.GetAdminBookings;

public sealed class Handler(BookingDbContext db)
    : Mediator.IQueryHandler<Request, ApiResponse<AdminBookingListResponse>>
{
    public async ValueTask<ApiResponse<AdminBookingListResponse>> Handle(Request req, CancellationToken ct)
    {
        var page     = Math.Max(1, req.Page);
        var pageSize = Math.Clamp(req.PageSize, 1, 100);

        var query = db.Bookings.AsNoTracking();

        // Status filter
        if (!string.IsNullOrWhiteSpace(req.Status) &&
            Enum.TryParse<BookingStatus>(req.Status, ignoreCase: true, out var status))
        {
            query = query.Where(b => b.Status == status);
        }

        // Search by GuestId or PropertyId (partial GUID)
        if (!string.IsNullOrWhiteSpace(req.Search) &&
            Guid.TryParse(req.Search, out var searchId))
        {
            query = query.Where(b => b.GuestId == searchId || b.PropertyId == searchId || b.HostId == searchId);
        }

        var total = await query.CountAsync(ct);

        query = (req.SortBy?.ToLower(), req.SortOrder?.ToLower()) switch
        {
            ("totalprice", "asc")  => query.OrderBy(b => b.TotalPrice),
            ("totalprice", _)      => query.OrderByDescending(b => b.TotalPrice),
            ("checkin", "asc")     => query.OrderBy(b => b.CheckIn),
            ("checkin", _)         => query.OrderByDescending(b => b.CheckIn),
            _                      => query.OrderByDescending(b => b.CreatedAt),
        };

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(b => new AdminBookingResponse(
                b.Id,
                b.PropertyId,
                b.HostId,
                b.GuestId,
                b.CountryCode,
                b.BookingMode,
                b.CheckIn,
                b.CheckOut,
                b.GuestCount,
                b.NightCount,
                b.TotalPrice,
                b.CurrencyCode,
                b.Status.ToString(),
                b.CreatedAt))
            .ToListAsync(ct);

        var totalPages = (int)Math.Ceiling(total / (double)pageSize);

        return ApiResponse<AdminBookingListResponse>.SuccessResult(
            new AdminBookingListResponse(items, total, page, pageSize, totalPages));
    }
}

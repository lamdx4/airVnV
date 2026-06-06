using Airbnb.BookingService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.BookingService.Features.GetAdminBookingById;

public sealed class Handler(BookingDbContext db)
    : Mediator.IQueryHandler<Request, ApiResponse<AdminBookingDetailResponse>>
{
    public async ValueTask<ApiResponse<AdminBookingDetailResponse>> Handle(Request req, CancellationToken ct)
    {
        var booking = await db.Bookings.AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == req.Id, ct)
            ?? throw new NotFoundException("Booking not found.");

        return ApiResponse<AdminBookingDetailResponse>.SuccessResult(new AdminBookingDetailResponse(
            booking.Id,
            booking.PropertyId,
            booking.HostId,
            booking.GuestId,
            booking.CountryCode,
            booking.BookingMode,
            booking.CheckIn,
            booking.CheckOut,
            booking.GuestCount,
            booking.NightCount,
            booking.BasePricePerNight,
            booking.CleaningFee,
            booking.ServiceFee,
            booking.TaxAmount,
            booking.TotalPrice,
            booking.CurrencyCode,
            booking.Status.ToString(),
            booking.CreatedAt));
    }
}

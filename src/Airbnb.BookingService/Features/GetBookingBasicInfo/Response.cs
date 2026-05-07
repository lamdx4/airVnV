namespace Airbnb.BookingService.Features.GetBookingBasicInfo;

public record Response(Guid BookingId, decimal TotalPrice, string CurrencyCode, string CountryCode, Guid GuestId);

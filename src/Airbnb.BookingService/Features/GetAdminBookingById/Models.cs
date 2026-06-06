using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.BookingService.Features.GetAdminBookingById;

public record Request(Guid Id) : Mediator.IQuery<ApiResponse<AdminBookingDetailResponse>>;

public record AdminBookingDetailResponse(
    Guid Id,
    Guid PropertyId,
    Guid HostId,
    Guid GuestId,
    string CountryCode,
    string BookingMode,
    DateOnly CheckIn,
    DateOnly CheckOut,
    int GuestCount,
    int NightCount,
    decimal BasePricePerNight,
    decimal CleaningFee,
    decimal ServiceFee,
    decimal TaxAmount,
    decimal TotalPrice,
    string CurrencyCode,
    string Status,
    DateTimeOffset CreatedAt
);

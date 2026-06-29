using Airbnb.BookingService.Domain;
using Airbnb.BookingService.Infrastructure;
using Airbnb.BookingService.Infrastructure.HttpClients;
using Airbnb.ServiceDefaults.Infrastructure;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.BookingService.Domain.Enums;

namespace Airbnb.BookingService.Features.CreateBooking;

public sealed class Handler(
    BookingDbContext db, 
    PropertyServiceClient propertyServiceClient,
    ILogger<Handler> logger) 
    : ICommandHandler<Request, Response>
{
    public async ValueTask<Response> Handle(Request req, CancellationToken ct)
    {
        if (req.UserId == Guid.Empty)
            throw new BusinessException("Missing UserId", "BOOKING_UNAUTHORIZED");

        // 1. Fetch Property and Pricing from PropertyService
        var propertyInfo = await propertyServiceClient.GetPropertyBasicInfoAsync(req.PropertyId, ct);
        if (propertyInfo == null)
            throw new BusinessException("Property not found or unavailable.", "BOOKING_PROPERTY_NOT_FOUND");

        var pricing = propertyInfo.Pricing;
        if (pricing == null)
            throw new BusinessException("Property pricing is missing.", "BOOKING_PRICING_ERROR");

        // 2. Check for overlapping bookings
        var isOverlapping = await db.Bookings.AnyAsync(b => 
            b.PropertyId == req.PropertyId &&
            b.Status != BookingStatus.Cancelled &&
            b.CheckIn < req.CheckOut && 
            b.CheckOut > req.CheckIn, ct);

        if (isOverlapping)
            throw new BusinessException("These dates are already booked.", "BOOKING_DATES_OVERLAPPING");

        // 3. Calculate Pricing
        var nightCount = req.CheckOut.DayNumber - req.CheckIn.DayNumber;
        if (nightCount <= 0)
            throw new BusinessException("CheckOut must be after CheckIn", "BOOKING_INVALID_DATES");

        var baseTotal = nightCount * pricing.BasePrice;
        
        // Calculate Weekend Premium
        decimal weekendPremiumTotal = 0;
        if (pricing.WeekendPremiumPercent > 0)
        {
            var premiumAmountPerNight = pricing.BasePrice * (pricing.WeekendPremiumPercent / 100m);
            for (var d = req.CheckIn; d < req.CheckOut; d = d.AddDays(1))
            {
                if (d.DayOfWeek == DayOfWeek.Friday || d.DayOfWeek == DayOfWeek.Saturday)
                {
                    weekendPremiumTotal += premiumAmountPerNight;
                }
            }
        }

        // Fetch Tax Rate from PropertyService Master Data (Internal API)
        decimal taxRate = 0m;
        if (!string.IsNullOrEmpty(propertyInfo.CountryCode))
        {
            var masterData = await propertyServiceClient.GetCountryMasterDataAsync(propertyInfo.CountryCode, ct);
            if (masterData != null && masterData.IsSupported)
            {
                // Simple assumption: Sum up all active tax rates (e.g. VAT + City Tax)
                taxRate = masterData.Taxes.Sum(t => t.Rate);
            }
        }

        // VAT is applied to (BasePrice + WeekendPremium + CleaningFee)
        // Platform ServiceFee is typically exempt
        var taxableAmount = baseTotal + weekendPremiumTotal + pricing.CleaningFee;
        var taxAmount = taxableAmount * taxRate;

        var totalPrice = taxableAmount + pricing.ServiceFee + taxAmount;

        var bookingMode = propertyInfo.BookingMode;
        if (bookingMode != BookingMode.InstantBook && bookingMode != BookingMode.RequestToBook)
        {
            bookingMode = BookingMode.InstantBook;
        }

        // 4. Create and Save Booking
        var booking = Booking.Create(
            req.PropertyId, 
            propertyInfo.HostId, 
            req.UserId, 
            propertyInfo.CountryCode,
            req.CheckIn, 
            req.CheckOut, 
            req.GuestCount,
            pricing.BasePrice, 
            pricing.CleaningFee, 
            pricing.ServiceFee, 
            taxAmount,
            totalPrice, 
            pricing.CurrencyCode,
            bookingMode);

        db.Bookings.Add(booking);
        await db.SaveChangesAsync(ct);

        logger.LogInformation("Booking {BookingId} created successfully for Property {PropertyId}", booking.Id, req.PropertyId);

        return new Response(booking.Id, booking.Status.ToString());
    }
}

using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.PropertyService.Domain.ValueObjects;
using Airbnb.PropertyService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.UpdateProperty;

public sealed class Handler(AppDbContext db)
    : ICommandHandler<Request, Response>
{
    public async ValueTask<Response> Handle(Request req, CancellationToken ct)
    {
        var property = await db.Properties
            .FirstOrDefaultAsync(p => p.Id == req.PropertyId && p.HostId == req.RequesterId, ct)
            ?? throw new NotFoundException("Property not found or access denied.");

        // Build partial value objects only if any field in that group changed
        Pricing? pricing = null;
        if (req.BasePrice.HasValue || req.CurrencyCode is not null ||
            req.CleaningFee.HasValue || req.ServiceFee.HasValue || req.WeekendPremiumPercent.HasValue)
        {
            pricing = new Pricing(
                req.BasePrice ?? property.Pricing.BasePrice,
                req.CurrencyCode ?? property.Pricing.CurrencyCode,
                req.CleaningFee ?? property.Pricing.CleaningFee,
                req.ServiceFee ?? property.Pricing.ServiceFee,
                req.WeekendPremiumPercent ?? property.Pricing.WeekendPremiumPercent);
        }

        PropertyCapacity? capacity = null;
        if (req.GuestCount.HasValue || req.BedroomCount.HasValue ||
            req.BedCount.HasValue || req.BathroomCount.HasValue)
        {
            capacity = new PropertyCapacity(
                req.GuestCount ?? property.Capacity.GuestCount,
                req.BedroomCount ?? property.Capacity.BedroomCount,
                req.BedCount ?? property.Capacity.BedCount,
                req.BathroomCount ?? property.Capacity.BathroomCount);
        }

        HouseRules? houseRules = null;
        if (req.AllowPets.HasValue || req.AllowSmoking.HasValue || req.AllowEvents.HasValue ||
            req.CheckInTime.HasValue || req.CheckOutTime.HasValue || req.FlexibleCheckOut.HasValue)
        {
            houseRules = new HouseRules(
                req.AllowPets ?? property.HouseRules.AllowPets,
                req.AllowSmoking ?? property.HouseRules.AllowSmoking,
                req.AllowEvents ?? property.HouseRules.AllowEvents,
                req.CheckInTime ?? property.HouseRules.CheckInTime,
                req.CheckOutTime ?? property.HouseRules.CheckOutTime,
                req.FlexibleCheckOut ?? property.HouseRules.FlexibleCheckOut);
        }

        property.UpdateCoreInfo(req.Title, req.Description, pricing, capacity, houseRules);
        await db.SaveChangesAsync(ct);

        return new Response(property.Id, property.UpdatedAt!.Value);
    }
}

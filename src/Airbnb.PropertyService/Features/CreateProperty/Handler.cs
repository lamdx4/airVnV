using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.PropertyService.Domain;
using Airbnb.PropertyService.Domain.ValueObjects;
using Airbnb.PropertyService.Infrastructure;
using Airbnb.PropertyService.Infrastructure.Messaging;

namespace Airbnb.PropertyService.Features.CreateProperty;

/// <summary>
/// Handler chứa toàn bộ business logic cho CreateProperty.
/// Endpoint không biết gì về DB, Domain, hay Messaging.
/// </summary>
public sealed class Handler(AppDbContext db, DomainEventPublisher publisher)
    : ICommandHandler<Request, Response>
{
    public async ValueTask<Response> Handle(Request req, CancellationToken ct)
    {
        string? admin1Code = req.Admin1Code?.Length > 10 ? null : req.Admin1Code;
        string? admin2Code = req.Admin2Code?.Length > 10 ? null : req.Admin2Code;

        // Sync-Matching ngầm với bảng admin_divisions nếu Frontend không gửi mã cứng lên
        if (req.SubDivisions != null)
        {
            var countryCode = req.CountryCode.ToUpperInvariant();

            // 1. Tìm kiếm Admin1 (Tỉnh/Bang)
            if (string.IsNullOrEmpty(admin1Code) &&
                (req.SubDivisions.TryGetValue("province", out var provinceName) || 
                 req.SubDivisions.TryGetValue("state", out provinceName) || 
                 req.SubDivisions.TryGetValue("admin1", out provinceName)))
            {
                var admin1 = await db.AdminDivisions
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.CountryCode == countryCode && 
                                              x.Level == 1 && 
                                              (x.NameLocal == provinceName || 
                                               x.NameEn == provinceName || 
                                               x.Aliases.Contains(provinceName)), ct);
                
                admin1Code = admin1?.Code;
            }

            // 2. Tìm kiếm Admin2 (Quận/Huyện/Phường/Thành phố con)
            if (string.IsNullOrEmpty(admin2Code) && !string.IsNullOrEmpty(admin1Code) &&
                (req.SubDivisions.TryGetValue("ward", out var wardName) || 
                 req.SubDivisions.TryGetValue("city", out wardName) || 
                 req.SubDivisions.TryGetValue("district", out wardName) || 
                 req.SubDivisions.TryGetValue("admin2", out wardName)))
            {
                var admin2 = await db.AdminDivisions
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.CountryCode == countryCode && 
                                              x.Level == 2 && 
                                              x.ParentCode == admin1Code && 
                                              (x.NameLocal == wardName || 
                                               x.NameEn == wardName || 
                                               x.Aliases.Contains(wardName)), ct);
                
                admin2Code = admin2?.Code;
            }
        }

        var pricing = new Pricing(
            req.BasePrice,
            req.CurrencyCode,
            req.CleaningFee,
            req.ServiceFee,
            req.WeekendPremiumPercent);

        var addressRaw = new AddressRaw(
            req.StreetAddress,
            req.Unit,
            req.PostalCode,
            SubDivisions: req.SubDivisions,
            Notes: new AddressNotes());

        var capacity = new PropertyCapacity(
            req.GuestCount,
            req.BedroomCount,
            req.BedCount,
            req.BathroomCount);

        var houseRules = new HouseRules(
            AllowPets: req.AllowPets,
            AllowSmoking: req.AllowSmoking,
            AllowEvents: req.AllowEvents,
            CheckInTime: req.CheckInTime,
            CheckOutTime: req.CheckOutTime,
            FlexibleCheckIn: false,
            FlexibleCheckOut: req.FlexibleCheckOut,
            CustomRules: req.CustomRules);

        var property = Property.Create(
            hostId: req.HostId,
            title: req.Title,
            description: req.Description,
            slug: req.Slug,
            latitude: req.Latitude,
            longitude: req.Longitude,
            countryCode: req.CountryCode,
            displayAddress: req.DisplayAddress,
            addressRaw: addressRaw,
            pricing: pricing,
            capacity: capacity,
            houseRules: houseRules,
            admin1Code: admin1Code,
            admin2Code: admin2Code);

        db.Properties.Add(property);

        // Dispatch domain events vào MassTransit Outbox trước SaveChanges
        await publisher.DispatchAsync(property.DomainEvents, ct);
        property.ClearDomainEvents();

        await db.SaveChangesAsync(ct);

        return new Response(property.Id, property.Slug);
    }
}

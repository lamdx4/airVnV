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
            SubDivisions: null,
            Notes: null);

        var capacity = new PropertyCapacity(
            req.GuestCount,
            req.BedroomCount,
            req.BedCount,
            req.BathroomCount);

        var houseRules = new HouseRules(
            req.AllowPets,
            req.AllowSmoking,
            req.AllowEvents,
            req.CheckInTime,
            req.CheckOutTime,
            req.FlexibleCheckOut);

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
            admin1Code: req.Admin1Code,
            admin2Code: req.Admin2Code);

        db.Properties.Add(property);

        // Dispatch domain events vào MassTransit Outbox trước SaveChanges
        await publisher.DispatchAsync(property.DomainEvents, ct);
        property.ClearDomainEvents();

        await db.SaveChangesAsync(ct);

        return new Response(property.Id, property.Slug);
    }
}

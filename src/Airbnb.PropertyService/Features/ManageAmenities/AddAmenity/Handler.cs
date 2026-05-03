using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.PropertyService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.ManageAmenities.AddAmenity;

public sealed class Handler(AppDbContext db)
    : ICommandHandler<Request, Response>
{
    public async ValueTask<Response> Handle(Request req, CancellationToken ct)
    {
        // 1. Verify Amenity exists in catalog
        var amenityExists = await db.Amenities.AnyAsync(a => a.Id == req.AmenityId, ct);
        if (!amenityExists)
            throw new NotFoundException("Amenity not found in catalog.");

        // 2. Get Property and add
        var property = await db.Properties
            .Include(p => p.PropertyAmenities)
            .FirstOrDefaultAsync(p => p.Id == req.PropertyId && p.HostId == req.RequesterId, ct)
            ?? throw new NotFoundException("Property not found or access denied.");

        property.AddAmenity(req.AmenityId, req.AdditionalInfo);
        await db.SaveChangesAsync(ct);

        return new Response(property.Id, req.AmenityId, "Amenity added successfully.");
    }
}

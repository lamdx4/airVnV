using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.PropertyService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.GetProperty;

public sealed class Handler(AppDbContext db) : IQueryHandler<Request, PropertyDto>
{
    public async ValueTask<PropertyDto> Handle(Request req, CancellationToken ct)
    {
        var property = await db.Properties
            .AsNoTracking()
            .Include(p => p.Images)
            .Include(p => p.PropertyAmenities)
            .Include(p => p.Availabilities)
            .FirstOrDefaultAsync(p => p.Id == req.PropertyId, ct);

        if (property == null)
            throw new NotFoundException("Property not found.");

        var amenityIds = property.PropertyAmenities.Select(pa => pa.AmenityId).ToList();
        var amenities = await db.Amenities
            .AsNoTracking()
            .Where(a => amenityIds.Contains(a.Id))
            .ToListAsync(ct);

        var imagesDto = property.Images.Select(i => new PropertyImageDto(i.Id, i.Url.ToString(), i.Type, i.DisplayOrder)).ToList();
        var amenitiesDto = property.PropertyAmenities.Select(pa => {
            var amenity = amenities.First(a => a.Id == pa.AmenityId);
            return new PropertyAmenityDto(
                pa.AmenityId, amenity.Name, amenity.IconCode ?? "", amenity.Category, pa.AdditionalInfo);
        }).ToList();
        var availabilitiesDto = property.Availabilities.Select(a => new PropertyAvailabilityDto(
            a.Id, a.StartDate, a.EndDate, a.Type, a.Note)).ToList();

        return new PropertyDto(
            property.Id,
            property.HostId,
            property.Title,
            property.Description,
            property.Slug,
            property.Status,
            property.Latitude,
            property.Longitude,
            property.DisplayAddress,
            property.CountryCode,
            property.Pricing,
            property.Capacity,
            property.HouseRules,
            imagesDto,
            amenitiesDto,
            availabilitiesDto,
            property.CreatedAt,
            property.UpdatedAt
        );
    }
}

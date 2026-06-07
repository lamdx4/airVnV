using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.PropertyService.Domain.Enums;
using Airbnb.PropertyService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.Admin.GetPropertyDetail;

public sealed class Handler(AppDbContext db) : IQueryHandler<Request, AdminPropertyDetailDto>
{
    public async ValueTask<AdminPropertyDetailDto> Handle(Request req, CancellationToken ct)
    {
        var property = await db.Properties
            .AsNoTracking()
            .Include(p => p.Images)
            .Include(p => p.PropertyAmenities)
            .FirstOrDefaultAsync(p => p.Id == req.PropertyId, ct);

        if (property == null)
            throw new NotFoundException("Property not found.");

        var amenityIds = property.PropertyAmenities.Select(pa => pa.AmenityId).ToList();

        var amenities = amenityIds.Count > 0
            ? await db.Amenities
                .AsNoTracking()
                .Where(a => amenityIds.Contains(a.Id))
                .ToListAsync(ct)
            : [];

        var imagesDto = property.Images
            .Select(i => new AdminPropertyImageDto(i.Url.ToString(), i.DisplayOrder))
            .ToList();

        var amenitiesDto = property.PropertyAmenities
            .Select(pa =>
            {
                var amenity = amenities.FirstOrDefault(a => a.Id == pa.AmenityId);
                return amenity != null
                    ? new AdminPropertyAmenityDto(amenity.Name, amenity.Category)
                    : new AdminPropertyAmenityDto("Unknown", "Other");
            })
            .ToList();

        var coverImageUrl = property.Images
            .Where(i => i.Type == ImageType.Cover)
            .Select(i => i.Url.ToString())
            .FirstOrDefault();

        return new AdminPropertyDetailDto(
            property.Id,
            property.HostId,
            property.Title,
            property.Description,
            property.DisplayAddress,
            property.AddressRaw.StreetAddress,
            (int)property.Type,
            (int)property.Status,
            property.Pricing.BasePrice,
            property.Capacity.GuestCount,
            property.Capacity.BedroomCount,
            property.Capacity.BathroomCount,
            property.AverageRating,
            property.ReviewCount,
            coverImageUrl,
            imagesDto,
            amenitiesDto,
            property.RejectionReason,
            property.SuspensionReason,
            property.CreatedAt,
            property.UpdatedAt
        );
    }
}

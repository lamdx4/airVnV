namespace Airbnb.PropertyService.Domain.Entities;

public class PropertyAmenity
{
    public Guid PropertyId { get; private set; }
    public Guid AmenityId { get; private set; }
    public string? AdditionalInfo { get; private set; }

    // Internal: chỉ Property aggregate được tạo, tránh rò rỉ invariant ra ngoài
    internal PropertyAmenity(Guid propertyId, Guid amenityId, string? additionalInfo)
    {
        PropertyId = propertyId;
        AmenityId = amenityId;
        AdditionalInfo = additionalInfo;
    }

    private PropertyAmenity() { } // EF Core
}

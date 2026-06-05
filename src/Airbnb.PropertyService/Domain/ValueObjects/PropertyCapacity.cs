using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Domain.ValueObjects;

public record PropertyCapacity
{
    public int GuestCount { get; init; }
    public int BedroomCount { get; init; }  // 0 ok (studio apartment)
    public int BedCount { get; init; }
    public int BathroomCount { get; init; }

    public PropertyCapacity(int guestCount, int bedroomCount, int bedCount, int bathroomCount)
    {
        if (guestCount <= 0) throw new BusinessException("GuestCount must be at least 1.", "PROPERTY_INVALID_CAPACITY");
        if (bedCount <= 0) throw new BusinessException("BedCount must be at least 1.", "PROPERTY_INVALID_CAPACITY");
        if (bathroomCount <= 0) throw new BusinessException("BathroomCount must be at least 1.", "PROPERTY_INVALID_CAPACITY");
        if (bedroomCount < 0) throw new BusinessException("BedroomCount cannot be negative.", "PROPERTY_INVALID_CAPACITY");

        GuestCount = guestCount;
        BedroomCount = bedroomCount;
        BedCount = bedCount;
        BathroomCount = bathroomCount;
    }
}

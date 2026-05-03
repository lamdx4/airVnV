namespace Airbnb.PropertyService.Domain.ValueObjects;

public record PropertyCapacity
{
    public int GuestCount { get; init; }
    public int BedroomCount { get; init; }  // 0 ok (studio apartment)
    public int BedCount { get; init; }
    public int BathroomCount { get; init; }

    public PropertyCapacity(int guestCount, int bedroomCount, int bedCount, int bathroomCount)
    {
        if (guestCount <= 0) throw new ArgumentException("GuestCount must be at least 1.");
        if (bedCount <= 0) throw new ArgumentException("BedCount must be at least 1.");
        if (bathroomCount <= 0) throw new ArgumentException("BathroomCount must be at least 1.");
        if (bedroomCount < 0) throw new ArgumentException("BedroomCount cannot be negative.");

        GuestCount = guestCount;
        BedroomCount = bedroomCount;
        BedCount = bedCount;
        BathroomCount = bathroomCount;
    }
}

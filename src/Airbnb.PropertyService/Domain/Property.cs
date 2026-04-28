namespace Airbnb.PropertyService.Domain;

public class Property
{
    public Guid Id { get; private set; }
    public Guid HostId { get; private set; }
    public string Name { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public decimal PricePerNight { get; private set; }
    public AddressVO Address { get; private set; } = default!;
    public DateTime CreatedAt { get; private set; }

    // EF Core private constructor
    private Property() { }

    public Property(Guid hostId, string name, string description, decimal pricePerNight, AddressVO address)
    {
        if (hostId == Guid.Empty) throw new ArgumentException("HostId cannot be empty");
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be empty");
        if (pricePerNight <= 0) throw new ArgumentException("Price must be greater than 0");
        if (address == null) throw new ArgumentNullException(nameof(address));

        Id = Guid.NewGuid();
        HostId = hostId;
        Name = name;
        Description = description;
        PricePerNight = pricePerNight;
        Address = address;
        CreatedAt = DateTime.UtcNow;
    }

    // Behavior Method (Rich Domain)
    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice <= 0) throw new ArgumentException("Price must be positive");
        PricePerNight = newPrice;
    }
}

public record AddressVO(
    string CountryCode,
    string City,
    string? StateProvince,
    string? Ward,
    string StreetLine1,
    string? StreetLine2,
    string? PostalCode,
    double Latitude,
    double Longitude
);

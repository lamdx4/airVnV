namespace Airbnb.SearchService.Domain;

public class PropertyDoc
{
    public Guid Id { get; set; }
    public Guid HostId { get; set; }
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public decimal PricePerNight { get; set; }
    public AddressVO Address { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
}

public class AddressVO
{
    public string CountryCode { get; set; } = default!;
    public string City { get; set; } = default!;
    public string? StateProvince { get; set; }
    public string? Ward { get; set; }
    public string StreetLine1 { get; set; } = default!;
    public string? StreetLine2 { get; set; }
    public string? PostalCode { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

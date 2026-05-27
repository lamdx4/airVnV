namespace Airbnb.SearchService.Domain;

public class PropertyDoc
{
    public Guid Id { get; set; }
    public Guid HostId { get; set; }
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public decimal BasePrice { get; set; }
    public decimal AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public AddressVO Address { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
}

public class AddressVO
{
    public string CountryCode { get; set; } = default!;
    public string? Admin1Code { get; set; }
    public string? Admin2Code { get; set; }
    public string DisplayAddress { get; set; } = default!;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

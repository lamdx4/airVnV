namespace Airbnb.PropertyService.Domain.Entities;

public class Amenity
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;
    public string Category { get; private set; } = default!;
    public string? IconCode { get; private set; }
}

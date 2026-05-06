namespace Airbnb.PropertyService.Domain.Entities;

public enum AvailabilityType
{
    Blocked = 0,
    Booked = 1
}

public class PropertyAvailability
{
    public Guid Id { get; private set; }
    public Guid PropertyId { get; private set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly EndDate { get; private set; }
    public AvailabilityType Type { get; private set; }
    public string? Note { get; private set; }

    private PropertyAvailability() { }

    public static PropertyAvailability Create(Guid propertyId, DateOnly startDate, DateOnly endDate, AvailabilityType type, string? note = null)
    {
        if (endDate < startDate) throw new ArgumentException("EndDate cannot be before StartDate.");

        return new PropertyAvailability
        {
            Id = Guid.CreateVersion7(),
            PropertyId = propertyId,
            StartDate = startDate,
            EndDate = endDate,
            Type = type,
            Note = note
        };
    }
}

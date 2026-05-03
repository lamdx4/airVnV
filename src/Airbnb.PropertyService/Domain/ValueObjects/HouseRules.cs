namespace Airbnb.PropertyService.Domain.ValueObjects;

public record HouseRules(
    bool AllowPets,
    bool AllowSmoking,
    bool AllowEvents,
    TimeOnly CheckInTime,
    TimeOnly CheckOutTime,
    bool FlexibleCheckIn = false,
    bool FlexibleCheckOut = false
);

using Mediator;
using Airbnb.PropertyService.Domain.ValueObjects;

namespace Airbnb.PropertyService.Features.GetPropertyBasicInfo;

public record Request(Guid PropertyId) : IQuery<Response>;

public record Response(Guid PropertyId, string Title, string Description, Guid HostId, Pricing Pricing, PropertyCapacity Capacity, HouseRules HouseRules, string CountryCode, Airbnb.PropertyService.Domain.Enums.PropertyType Type, Airbnb.PropertyService.Domain.Enums.BookingMode BookingMode);

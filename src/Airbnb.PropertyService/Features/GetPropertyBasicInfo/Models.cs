using Mediator;
using Airbnb.PropertyService.Domain.ValueObjects;

namespace Airbnb.PropertyService.Features.GetPropertyBasicInfo;

public record Request(Guid PropertyId) : IQuery<Response>;

public record Response(Guid PropertyId, string Title, Guid HostId, Pricing Pricing, string CountryCode, Airbnb.PropertyService.Domain.Enums.BookingMode BookingMode);

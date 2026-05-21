using Mediator;

namespace Airbnb.PropertyService.Features.GetSupportedCountries;

public record SupportedCountryDto(
    string Code,
    string Name,
    string NativeCurrency,
    double DefaultLatitude,
    double DefaultLongitude
);

public record Request : IQuery<List<SupportedCountryDto>>;

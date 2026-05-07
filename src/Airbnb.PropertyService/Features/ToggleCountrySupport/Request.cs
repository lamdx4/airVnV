using Mediator;

namespace Airbnb.PropertyService.Features.ToggleCountrySupport;

public record Request(string CountryCode) : ICommand<Response>;
public record Response(string CountryCode, bool IsSupported);

using Mediator;

namespace Airbnb.PropertyService.Features.CreateTax;

public record Request(string CountryCode, string Type, decimal Rate, bool IsActive) : ICommand<Response>;
public record Response(Guid Id, string CountryCode, string Type, decimal Rate, bool IsActive);

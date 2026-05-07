using Mediator;

namespace Airbnb.PropertyService.Features.CreatePaymentGateway;

public record Request(string CountryCode, string Provider, string[] SupportedCurrencies, bool IsActive) : ICommand<Response>;
public record Response(Guid Id, string CountryCode, string Provider, string[] SupportedCurrencies, bool IsActive);

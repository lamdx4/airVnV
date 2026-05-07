using Mediator;

namespace Airbnb.PropertyService.Features.GetCountryMasterData;

public record TaxDto(string Type, decimal Rate);
public record PaymentGatewayDto(string Provider, string[] SupportedCurrencies);

public record CountryMasterDataDto(
    string CountryCode,
    string Name,
    string NativeCurrency,
    bool IsSupported,
    List<TaxDto> Taxes,
    List<PaymentGatewayDto> PaymentGateways
);

public record Request(string CountryCode) : IQuery<CountryMasterDataDto>;

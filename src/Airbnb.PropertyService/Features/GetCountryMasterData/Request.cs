using Airbnb.PropertyService.Domain.Entities;
using Mediator;

namespace Airbnb.PropertyService.Features.GetCountryMasterData;

public record TaxDto(string Type, decimal Rate);
public record PaymentGatewayDto(string Provider, string[] SupportedCurrencies);

public record CountryMasterDataDto(
    string CountryCode,
    string Name,
    string NativeCurrency,
    bool IsSupported,
    double DefaultLatitude,
    double DefaultLongitude,
    List<TaxDto> Taxes,
    List<PaymentGatewayDto> PaymentGateways,
    List<AddressFieldConfig>? AddressFormConfig
);

public record Request(string CountryCode) : IQuery<CountryMasterDataDto>;

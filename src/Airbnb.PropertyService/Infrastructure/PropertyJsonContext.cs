using System.Text.Json.Serialization;
using Airbnb.PropertyService.Features.CreateProperty;

namespace Airbnb.PropertyService.Infrastructure;

[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
[JsonSerializable(typeof(Airbnb.PropertyService.Features.CreateProperty.Request), TypeInfoPropertyName = "CreatePropertyRequest")]
[JsonSerializable(typeof(Airbnb.PropertyService.Features.CreateProperty.Response), TypeInfoPropertyName = "CreatePropertyResponse")]
[JsonSerializable(typeof(Airbnb.PropertyService.Features.GetProperty.Request), TypeInfoPropertyName = "GetPropertyRequest")]
[JsonSerializable(typeof(Airbnb.ServiceDefaults.Infrastructure.ApiResponse<Airbnb.PropertyService.Features.GetProperty.PropertyDto>), TypeInfoPropertyName = "ApiResponseGetProperty")]
[JsonSerializable(typeof(Airbnb.PropertyService.Features.GetCountryMasterData.Request), TypeInfoPropertyName = "GetCountryMasterDataRequest")]
[JsonSerializable(typeof(Airbnb.ServiceDefaults.Infrastructure.ApiResponse<Airbnb.PropertyService.Features.GetCountryMasterData.CountryMasterDataDto>), TypeInfoPropertyName = "ApiResponseGetCountryMasterData")]
[JsonSerializable(typeof(List<Airbnb.PropertyService.Domain.Entities.AddressFieldConfig>), TypeInfoPropertyName = "AddressFieldConfigList")]
[JsonSerializable(typeof(Airbnb.PropertyService.Domain.Entities.AddressFieldConfig), TypeInfoPropertyName = "AddressFieldConfig")]
[JsonSerializable(typeof(Airbnb.PropertyService.Domain.ValueObjects.AddressRaw), TypeInfoPropertyName = "AddressRaw")]
[JsonSerializable(typeof(Airbnb.PropertyService.Domain.ValueObjects.AddressNotes), TypeInfoPropertyName = "AddressNotes")]
[JsonSerializable(typeof(Airbnb.PropertyService.Domain.ValueObjects.HouseRules), TypeInfoPropertyName = "HouseRules")]
[JsonSerializable(typeof(System.Collections.Generic.List<string>), TypeInfoPropertyName = "StringList")]
[JsonSerializable(typeof(Dictionary<string, string>), TypeInfoPropertyName = "StringDictionary")]

[JsonSerializable(typeof(Airbnb.PropertyService.Features.ToggleCountrySupport.Request), TypeInfoPropertyName = "ToggleCountrySupportRequest")]
[JsonSerializable(typeof(Airbnb.PropertyService.Features.ToggleCountrySupport.Response), TypeInfoPropertyName = "ToggleCountrySupportResponse")]
[JsonSerializable(typeof(Airbnb.ServiceDefaults.Infrastructure.ApiResponse<Airbnb.PropertyService.Features.ToggleCountrySupport.Response>), TypeInfoPropertyName = "ApiResponseToggleCountrySupport")]

[JsonSerializable(typeof(Airbnb.PropertyService.Features.CreateTax.Request), TypeInfoPropertyName = "CreateTaxRequest")]
[JsonSerializable(typeof(Airbnb.PropertyService.Features.CreateTax.Response), TypeInfoPropertyName = "CreateTaxResponse")]
[JsonSerializable(typeof(Airbnb.ServiceDefaults.Infrastructure.ApiResponse<Airbnb.PropertyService.Features.CreateTax.Response>), TypeInfoPropertyName = "ApiResponseCreateTax")]

[JsonSerializable(typeof(Airbnb.PropertyService.Features.CreatePaymentGateway.Request), TypeInfoPropertyName = "CreatePaymentGatewayRequest")]
[JsonSerializable(typeof(Airbnb.PropertyService.Features.CreatePaymentGateway.Response), TypeInfoPropertyName = "CreatePaymentGatewayResponse")]
[JsonSerializable(typeof(Airbnb.ServiceDefaults.Infrastructure.ApiResponse<Airbnb.PropertyService.Features.CreatePaymentGateway.Response>), TypeInfoPropertyName = "ApiResponseCreatePaymentGateway")]

[JsonSerializable(typeof(Airbnb.PropertyService.Features.GetMyProperties.Request), TypeInfoPropertyName = "GetMyPropertiesRequest")]
[JsonSerializable(typeof(Airbnb.PropertyService.Features.GetMyProperties.PropertyResponse), TypeInfoPropertyName = "GetMyPropertiesPropertyResponse")]
[JsonSerializable(typeof(Airbnb.PropertyService.Features.GetMyProperties.PagedResponse<Airbnb.PropertyService.Features.GetMyProperties.PropertyResponse>), TypeInfoPropertyName = "GetMyPropertiesPagedResponse")]
[JsonSerializable(typeof(Airbnb.ServiceDefaults.Infrastructure.ApiResponse<Airbnb.PropertyService.Features.GetMyProperties.PagedResponse<Airbnb.PropertyService.Features.GetMyProperties.PropertyResponse>>), TypeInfoPropertyName = "ApiResponseGetMyPropertiesPaged")]
[JsonSerializable(typeof(Airbnb.PropertyService.Features.GetSupportedCountries.Request), TypeInfoPropertyName = "GetSupportedCountriesRequest")]
[JsonSerializable(typeof(Airbnb.PropertyService.Features.GetSupportedCountries.SupportedCountryDto), TypeInfoPropertyName = "SupportedCountryDto")]
[JsonSerializable(typeof(List<Airbnb.PropertyService.Features.GetSupportedCountries.SupportedCountryDto>), TypeInfoPropertyName = "SupportedCountryDtoList")]
[JsonSerializable(typeof(Airbnb.ServiceDefaults.Infrastructure.ApiResponse<List<Airbnb.PropertyService.Features.GetSupportedCountries.SupportedCountryDto>>), TypeInfoPropertyName = "ApiResponseGetSupportedCountries")]

[JsonSerializable(typeof(FastEndpoints.ErrorResponse))] // Quan trọng cho validation errors
internal partial class PropertyJsonContext : JsonSerializerContext { }

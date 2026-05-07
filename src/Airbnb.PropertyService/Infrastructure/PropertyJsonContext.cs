using System.Text.Json.Serialization;
using Airbnb.PropertyService.Features.CreateProperty;

namespace Airbnb.PropertyService.Infrastructure;

[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
[JsonSerializable(typeof(Airbnb.PropertyService.Features.CreateProperty.Request), TypeInfoPropertyName = "CreatePropertyRequest")]
[JsonSerializable(typeof(Airbnb.PropertyService.Features.CreateProperty.Response), TypeInfoPropertyName = "CreatePropertyResponse")]
[JsonSerializable(typeof(Airbnb.PropertyService.Features.GetProperty.Request), TypeInfoPropertyName = "GetPropertyRequest")]
[JsonSerializable(typeof(Airbnb.ServiceDefaults.Infrastructure.ApiResponse<Airbnb.PropertyService.Features.GetProperty.PropertyDto>))]
[JsonSerializable(typeof(Airbnb.PropertyService.Features.GetCountryMasterData.Request), TypeInfoPropertyName = "GetCountryMasterDataRequest")]
[JsonSerializable(typeof(Airbnb.ServiceDefaults.Infrastructure.ApiResponse<Airbnb.PropertyService.Features.GetCountryMasterData.CountryMasterDataDto>))]

[JsonSerializable(typeof(Airbnb.PropertyService.Features.ToggleCountrySupport.Request), TypeInfoPropertyName = "ToggleCountrySupportRequest")]
[JsonSerializable(typeof(Airbnb.ServiceDefaults.Infrastructure.ApiResponse<Airbnb.PropertyService.Features.ToggleCountrySupport.Response>), TypeInfoPropertyName = "ApiResponseToggleCountrySupportResponse")]
[JsonSerializable(typeof(Airbnb.PropertyService.Features.CreateTax.Request), TypeInfoPropertyName = "CreateTaxRequest")]
[JsonSerializable(typeof(Airbnb.ServiceDefaults.Infrastructure.ApiResponse<Airbnb.PropertyService.Features.CreateTax.Response>), TypeInfoPropertyName = "ApiResponseCreateTaxResponse")]
[JsonSerializable(typeof(Airbnb.PropertyService.Features.CreatePaymentGateway.Request), TypeInfoPropertyName = "CreatePaymentGatewayRequest")]
[JsonSerializable(typeof(Airbnb.ServiceDefaults.Infrastructure.ApiResponse<Airbnb.PropertyService.Features.CreatePaymentGateway.Response>), TypeInfoPropertyName = "ApiResponseCreatePaymentGatewayResponse")]

[JsonSerializable(typeof(FastEndpoints.ErrorResponse))] // Quan trọng cho validation errors
internal partial class PropertyJsonContext : JsonSerializerContext { }

using System.Net.Http.Json;
using Microsoft.Extensions.Caching.Memory;

namespace Airbnb.BookingService.Infrastructure.HttpClients;

public record PricingDto(decimal BasePrice, string CurrencyCode, decimal CleaningFee, decimal ServiceFee, decimal WeekendPremiumPercent);
public record PropertyBasicInfoResponse(Guid PropertyId, string Title, Guid HostId, PricingDto Pricing, string CountryCode);

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

public class PropertyServiceClient(HttpClient httpClient, Microsoft.Extensions.Caching.Memory.IMemoryCache cache)
{
    private static readonly TimeSpan MasterDataCacheDuration = TimeSpan.FromHours(24);
    public async Task<PropertyBasicInfoResponse?> GetPropertyBasicInfoAsync(Guid propertyId, CancellationToken ct = default)
    {
        var wrapper = await httpClient.GetFromJsonAsync<ApiResponseWrapper<PropertyBasicInfoResponse>>($"/api/properties/{propertyId}/basic-info", ct);
        return wrapper?.Data;
    }

    public async Task<CountryMasterDataDto?> GetCountryMasterDataAsync(string countryCode, CancellationToken ct = default)
    {
        var cacheKey = $"masterdata_country_{countryCode.ToUpperInvariant()}";
        
        if (cache.TryGetValue(cacheKey, out CountryMasterDataDto? cachedData))
            return cachedData;

        var wrapper = await httpClient.GetFromJsonAsync<ApiResponseWrapper<CountryMasterDataDto>>($"/api/internal/master-data/countries/{countryCode}", ct);
        
        if (wrapper?.Data != null)
        {
            cache.Set(cacheKey, wrapper.Data, MasterDataCacheDuration);
        }

        return wrapper?.Data;
    }

    public void InvalidateCountryMasterDataCache(string countryCode)
    {
        var cacheKey = $"masterdata_country_{countryCode.ToUpperInvariant()}";
        cache.Remove(cacheKey);
    }
}

public record ApiResponseWrapper<T>(T? Data, string? Message, bool Success, string? ErrorCode);

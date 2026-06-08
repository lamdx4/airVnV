using System.Net.Http.Json;
using Microsoft.Extensions.Caching.Memory;

namespace Airbnb.PaymentService.Infrastructure.HttpClients;

public record TaxDto(string Type, decimal Rate);
public record PaymentGatewayDto(string Provider, string[] SupportedCurrencies, bool IsActive);
public record CountryMasterDataDto(
    string CountryCode,
    string Name,
    bool IsSupported,
    List<TaxDto> Taxes,
    List<PaymentGatewayDto> PaymentGateways
);

public class PropertyServiceClient(HttpClient httpClient, IMemoryCache cache)
{
    private static readonly TimeSpan MasterDataCacheDuration = TimeSpan.FromMinutes(10);
    
    public async Task<CountryMasterDataDto?> GetCountryMasterDataAsync(string countryCode, CancellationToken ct = default)
    {
        var cacheKey = $"masterdata_country_{countryCode.ToUpperInvariant()}";

        if (cache.TryGetValue(cacheKey, out CountryMasterDataDto? cachedData))
            return cachedData;

        using var response = await httpClient.GetAsync($"/api/internal/master-data/countries/{countryCode}", ct);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            cache.Set(cacheKey, (CountryMasterDataDto?)null, MasterDataCacheDuration);
            return null;
        }
        response.EnsureSuccessStatusCode();

        var wrapper = await response.Content.ReadFromJsonAsync<ApiResponseWrapper<CountryMasterDataDto>>(cancellationToken: ct);
        if (wrapper?.Data != null)
        {
            cache.Set(cacheKey, wrapper.Data, MasterDataCacheDuration);
        }

        return wrapper?.Data;
    }
}

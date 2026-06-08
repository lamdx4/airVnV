using Airbnb.PaymentService.Infrastructure.HttpClients;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Infrastructure.PaymentGateways;

public class PaymentProviderResolver(
    PropertyServiceClient propertyServiceClient,
    IEnumerable<IPaymentProvider> providers,
    ILogger<PaymentProviderResolver> logger)
{
    // Hardcoded fallback when master-data endpoint is unavailable.
    private static readonly Dictionary<string, string> DefaultProviderByCountry = new(StringComparer.OrdinalIgnoreCase)
    {
        ["VN"] = "VNPay",
    };

    public async Task<IPaymentProvider> ResolveAsync(string countryCode, CancellationToken ct)
    {
        var masterData = await propertyServiceClient.GetCountryMasterDataAsync(countryCode, ct);

        string? providerName = null;

        if (masterData != null && masterData.IsSupported)
        {
            providerName = masterData.PaymentGateways.FirstOrDefault(g => g.IsActive)?.Provider;
        }

        // Fallback when master-data unavailable or no active gateway listed.
        if (string.IsNullOrEmpty(providerName) && DefaultProviderByCountry.TryGetValue(countryCode, out var fallback))
        {
            logger.LogWarning("Master-data missing for {CountryCode}; falling back to default provider {Provider}", countryCode, fallback);
            providerName = fallback;
        }

        if (string.IsNullOrEmpty(providerName))
        {
            logger.LogWarning("Country {CountryCode} is not supported for payments", countryCode);
            throw new BusinessException($"Country {countryCode} is not supported for payments", "PAYMENT_COUNTRY_NOT_SUPPORTED");
        }

        var provider = providers.FirstOrDefault(p => p.ProviderName.Equals(providerName, StringComparison.OrdinalIgnoreCase));

        if (provider == null)
        {
            logger.LogError("Payment provider implementation for {ProviderName} not found", providerName);
            throw new BusinessException($"Payment provider {providerName} not implemented", "PAYMENT_PROVIDER_NOT_IMPLEMENTED");
        }

        return provider;
    }
}

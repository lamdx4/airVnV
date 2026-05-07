using Airbnb.PaymentService.Infrastructure.HttpClients;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Infrastructure.PaymentGateways;

public class PaymentProviderResolver(
    PropertyServiceClient propertyServiceClient,
    IEnumerable<IPaymentProvider> providers,
    ILogger<PaymentProviderResolver> logger)
{
    public async Task<IPaymentProvider> ResolveAsync(string countryCode, CancellationToken ct)
    {
        var masterData = await propertyServiceClient.GetCountryMasterDataAsync(countryCode, ct);
        
        if (masterData == null || !masterData.IsSupported)
        {
            logger.LogWarning("Country {CountryCode} is not supported for payments", countryCode);
            throw new BusinessException($"Country {countryCode} is not supported for payments", "PAYMENT_COUNTRY_NOT_SUPPORTED");
        }

        // Find the first active gateway
        var activeGateway = masterData.PaymentGateways.FirstOrDefault(g => g.IsActive);
        
        if (activeGateway == null)
        {
            logger.LogWarning("No active payment gateway found for country {CountryCode}", countryCode);
            throw new BusinessException($"No active payment gateway found for {countryCode}", "PAYMENT_GATEWAY_NOT_FOUND");
        }

        var provider = providers.FirstOrDefault(p => p.ProviderName.Equals(activeGateway.Provider, StringComparison.OrdinalIgnoreCase));
        
        if (provider == null)
        {
            logger.LogError("Payment provider implementation for {ProviderName} not found", activeGateway.Provider);
            throw new BusinessException($"Payment provider {activeGateway.Provider} not implemented", "PAYMENT_PROVIDER_NOT_IMPLEMENTED");
        }

        return provider;
    }
}

using Airbnb.SharedKernel.Domain;

namespace Airbnb.PropertyService.Domain.Entities;

public class PaymentGateway : AggregateRoot
{
    public Guid Id { get; private set; }
    public string CountryCode { get; private set; } = default!;
    public string Provider { get; private set; } = default!;
    public string[] SupportedCurrencies { get; private set; } = Array.Empty<string>();
    public bool IsActive { get; private set; }

    private PaymentGateway() { } // EF Core

    public static PaymentGateway Create(string countryCode, string provider, string[] supportedCurrencies, bool isActive = true)
    {
        return new PaymentGateway
        {
            Id = Guid.CreateVersion7(),
            CountryCode = countryCode.ToUpperInvariant(),
            Provider = provider,
            SupportedCurrencies = supportedCurrencies.Select(c => c.ToUpperInvariant()).ToArray(),
            IsActive = isActive
        };
    }

    public void UpdateCurrencies(string[] currencies)
    {
        SupportedCurrencies = currencies.Select(c => c.ToUpperInvariant()).ToArray();
    }

    public void ToggleActive()
    {
        IsActive = !IsActive;
    }
}

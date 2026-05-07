using Airbnb.SharedKernel.Domain;

namespace Airbnb.PropertyService.Domain.Entities;

public class Country : AggregateRoot
{
    public string Code { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public string NativeCurrency { get; private set; } = default!;
    public bool IsSupported { get; private set; }

    // Navigation properties for relationships
    private readonly List<Tax> _taxes = new();
    public IReadOnlyCollection<Tax> Taxes => _taxes.AsReadOnly();

    private readonly List<PaymentGateway> _paymentGateways = new();
    public IReadOnlyCollection<PaymentGateway> PaymentGateways => _paymentGateways.AsReadOnly();

    private Country() { } // EF Core

    public static Country Create(string code, string name, string nativeCurrency, bool isSupported = false)
    {
        return new Country
        {
            Code = code.ToUpperInvariant(),
            Name = name,
            NativeCurrency = nativeCurrency.ToUpperInvariant(),
            IsSupported = isSupported
        };
    }

    public void ToggleSupport()
    {
        IsSupported = !IsSupported;
    }
}

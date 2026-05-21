using Airbnb.SharedKernel.Domain;

namespace Airbnb.PropertyService.Domain.Entities;

public record AddressFieldConfig
{
    public string Id { get; init; } = default!;
    public string Label { get; init; } = default!;
    public string Type { get; init; } = "text";
    public bool IsRequired { get; init; }
    public List<string> PhotonKeys { get; init; } = new();
}

public class Country : AggregateRoot
{
    public string Code { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public string NativeCurrency { get; private set; } = default!;
    public bool IsSupported { get; private set; }
    public double DefaultLatitude { get; private set; }
    public double DefaultLongitude { get; private set; }
    public List<AddressFieldConfig>? AddressFormConfig { get; private set; }

    // Navigation properties for relationships
    private readonly List<Tax> _taxes = new();
    public IReadOnlyCollection<Tax> Taxes => _taxes.AsReadOnly();

    private readonly List<PaymentGateway> _paymentGateways = new();
    public IReadOnlyCollection<PaymentGateway> PaymentGateways => _paymentGateways.AsReadOnly();

    private Country() { } // EF Core

    public static Country Create(string code, string name, string nativeCurrency, double defaultLat = 0, double defaultLng = 0, bool isSupported = false)
    {
        return new Country
        {
            Code = code.ToUpperInvariant(),
            Name = name,
            NativeCurrency = nativeCurrency.ToUpperInvariant(),
            DefaultLatitude = defaultLat,
            DefaultLongitude = defaultLng,
            IsSupported = isSupported
        };
    }

    public void ToggleSupport()
    {
        IsSupported = !IsSupported;
    }

    public void UpdateAddressFormConfig(List<AddressFieldConfig> config)
    {
        AddressFormConfig = config;
    }

    public void UpdateDefaultCoordinates(double lat, double lng)
    {
        DefaultLatitude = lat;
        DefaultLongitude = lng;
    }
}


using Airbnb.SharedKernel.Domain;

namespace Airbnb.PropertyService.Domain.Entities;

public class Tax : AggregateRoot
{
    public Guid Id { get; private set; }
    public string CountryCode { get; private set; } = default!;
    public string Type { get; private set; } = default!;
    public decimal Rate { get; private set; }
    public bool IsActive { get; private set; }

    private Tax() { } // EF Core

    public static Tax Create(string countryCode, string type, decimal rate, bool isActive = true)
    {
        return new Tax
        {
            Id = Guid.CreateVersion7(),
            CountryCode = countryCode.ToUpperInvariant(),
            Type = type,
            Rate = rate,
            IsActive = isActive
        };
    }

    public void UpdateRate(decimal newRate)
    {
        Rate = newRate;
    }

    public void ToggleActive()
    {
        IsActive = !IsActive;
    }
}

using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Domain.ValueObjects;

public record Pricing
{
    public decimal BasePrice { get; init; }
    public string CurrencyCode { get; init; } = default!;  // ISO 4217
    public decimal CleaningFee { get; init; }
    public decimal ServiceFee { get; init; }
    public decimal WeekendPremiumPercent { get; init; }    // 15 = +15%, max 500%

    public Pricing(
        decimal basePrice,
        string currencyCode,
        decimal cleaningFee,
        decimal serviceFee,
        decimal weekendPremiumPercent)
    {
        if (basePrice <= 0) throw new BusinessException("BasePrice must be greater than 0.", "PROPERTY_INVALID_PRICE");
        if (string.IsNullOrWhiteSpace(currencyCode) || currencyCode.Length != 3)
            throw new BusinessException("CurrencyCode must be ISO 4217 (3 chars).", "PROPERTY_INVALID_PRICE");
        if (cleaningFee < 0) throw new BusinessException("CleaningFee cannot be negative.", "PROPERTY_INVALID_FEES");
        if (serviceFee < 0) throw new BusinessException("ServiceFee cannot be negative.", "PROPERTY_INVALID_FEES");
        if (weekendPremiumPercent < 0 || weekendPremiumPercent > 500)
            throw new BusinessException("WeekendPremiumPercent must be between 0 and 500.", "PROPERTY_INVALID_WEEKEND_PREMIUM");

        BasePrice = basePrice;
        CurrencyCode = currencyCode.ToUpperInvariant();
        CleaningFee = cleaningFee;
        ServiceFee = serviceFee;
        WeekendPremiumPercent = weekendPremiumPercent;
    }
}

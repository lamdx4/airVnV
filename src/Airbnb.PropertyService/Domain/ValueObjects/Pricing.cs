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
        if (basePrice <= 0) throw new ArgumentException("BasePrice must be greater than 0.");
        if (string.IsNullOrWhiteSpace(currencyCode) || currencyCode.Length != 3)
            throw new ArgumentException("CurrencyCode must be ISO 4217 (3 chars).");
        if (cleaningFee < 0) throw new ArgumentException("CleaningFee cannot be negative.");
        if (serviceFee < 0) throw new ArgumentException("ServiceFee cannot be negative.");
        if (weekendPremiumPercent < 0 || weekendPremiumPercent > 500)
            throw new ArgumentException("WeekendPremiumPercent must be between 0 and 500.");

        BasePrice = basePrice;
        CurrencyCode = currencyCode.ToUpperInvariant();
        CleaningFee = cleaningFee;
        ServiceFee = serviceFee;
        WeekendPremiumPercent = weekendPremiumPercent;
    }
}

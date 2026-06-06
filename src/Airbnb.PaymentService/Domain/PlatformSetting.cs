using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Domain;

/// <summary>
/// Singleton entity that stores platform-wide payment configuration.
/// Only one row exists; identified by a fixed Id.
/// </summary>
public class PlatformSetting
{
    public static readonly Guid SingletonId =
        Guid.Parse("11111111-1111-1111-1111-111111111111");

    public Guid Id { get; private set; }
    public decimal PlatformFeePercent { get; private set; }
    public decimal MinPayoutAmount { get; private set; }
    public string DefaultCurrency { get; private set; } = default!;
    public DateTimeOffset UpdatedAt { get; private set; }
    public string? UpdatedBy { get; private set; }

    private PlatformSetting() { } // EF Core

    public static PlatformSetting CreateDefault() => new()
    {
        Id = SingletonId,
        PlatformFeePercent = 10m,
        MinPayoutAmount = 50m,
        DefaultCurrency = "USD",
        UpdatedAt = DateTimeOffset.UtcNow,
        UpdatedBy = null,
    };

    public void Update(decimal platformFeePercent, decimal minPayoutAmount, string defaultCurrency, string? updatedBy)
    {
        if (platformFeePercent < 0 || platformFeePercent > 100)
            throw new BusinessException("PlatformFeePercent must be between 0 and 100.", "PLATFORM_FEE_OUT_OF_RANGE");
        if (minPayoutAmount < 0)
            throw new BusinessException("MinPayoutAmount cannot be negative.", "MIN_PAYOUT_INVALID");
        if (string.IsNullOrWhiteSpace(defaultCurrency) || defaultCurrency.Length != 3)
            throw new BusinessException("DefaultCurrency must be a 3-letter ISO code.", "CURRENCY_INVALID");

        PlatformFeePercent = platformFeePercent;
        MinPayoutAmount = minPayoutAmount;
        DefaultCurrency = defaultCurrency.ToUpperInvariant();
        UpdatedAt = DateTimeOffset.UtcNow;
        UpdatedBy = updatedBy;
    }
}

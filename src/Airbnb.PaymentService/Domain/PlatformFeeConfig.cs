namespace Airbnb.PaymentService.Domain;

public class PlatformFeeConfig
{
    public Guid Id { get; private set; }
    public decimal FeePercentage { get; private set; }
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }
    public Guid ChangedBy { get; private set; }
    public decimal? PreviousValue { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private PlatformFeeConfig() { } // EF Core

    public static PlatformFeeConfig Create(decimal feePercentage, Guid changedBy, decimal? previousValue = null, string? description = null)
    {
        if (feePercentage < 0 || feePercentage > 50)
            throw new ArgumentException("Fee percentage must be between 0% and 50%.");
        if (changedBy == Guid.Empty)
            throw new ArgumentException("ChangedBy cannot be empty.");

        return new PlatformFeeConfig
        {
            Id = Guid.CreateVersion7(),
            FeePercentage = feePercentage,
            Description = description,
            IsActive = true,
            ChangedBy = changedBy,
            PreviousValue = previousValue,
            CreatedAt = DateTimeOffset.UtcNow,
        };
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}

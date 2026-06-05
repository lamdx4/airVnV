using Airbnb.PaymentService.Domain;
using Airbnb.PaymentService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.PaymentService.Features.CreateAdminPlatformFee;

public sealed class Handler(PaymentDbContext db, ILogger<Handler> logger)
    : ICommandHandler<Command, PlatformFeeCreateResponse>
{
    public async ValueTask<PlatformFeeCreateResponse> Handle(Command req, CancellationToken ct)
    {
        // Validate fee range
        if (req.FeePercentage < 0 || req.FeePercentage > 50)
            throw new BusinessException("Fee percentage must be between 0% and 50%", "PLATFORM_FEE_INVALID_RANGE");

        // Get current active config for previous value
        var currentActive = await db.PlatformFeeConfigs
            .Where(c => c.IsActive)
            .OrderByDescending(c => c.CreatedAt)
            .FirstOrDefaultAsync(ct);

        var previousValue = currentActive?.FeePercentage;

        // Deactivate all current configs
        if (currentActive is not null)
            currentActive.Deactivate();

        // Create new config
        var newConfig = PlatformFeeConfig.Create(
            req.FeePercentage,
            req.ChangedBy,
            previousValue,
            req.Description
        );

        db.PlatformFeeConfigs.Add(newConfig);
        await db.SaveChangesAsync(ct);

        logger.LogInformation("Platform fee updated to {FeePercentage}% by {ChangedBy} (previous: {PreviousValue}%)",
            req.FeePercentage, req.ChangedBy, previousValue);

        return new PlatformFeeCreateResponse(
            newConfig.Id,
            newConfig.FeePercentage,
            newConfig.Description,
            previousValue,
            newConfig.CreatedAt
        );
    }
}

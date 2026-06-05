using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.PaymentService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Features.GetAdminPlatformFeeCurrent;

public sealed class Handler(PaymentDbContext db) : IQueryHandler<Request, PlatformFeeCurrentResponse>
{
    public async ValueTask<PlatformFeeCurrentResponse> Handle(Request req, CancellationToken ct)
    {
        var config = await db.PlatformFeeConfigs
            .AsNoTracking()
            .Where(c => c.IsActive)
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new PlatformFeeCurrentResponse(
                c.Id,
                c.FeePercentage,
                c.Description,
                c.ChangedBy,
                c.PreviousValue,
                c.CreatedAt
            ))
            .FirstOrDefaultAsync(ct);

        if (config is null)
            throw new BusinessException("No active platform fee configuration found", "PLATFORM_FEE_NOT_FOUND");

        return config;
    }
}

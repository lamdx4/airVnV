using Airbnb.PaymentService.Domain;
using Airbnb.PaymentService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.PaymentService.Features.ApprovePayout;

public sealed class Handler(PaymentDbContext db, ILogger<Handler> logger)
    : ICommandHandler<Command, PayoutActionResponse>
{
    public async ValueTask<PayoutActionResponse> Handle(Command req, CancellationToken ct)
    {
        var payout = await db.Payouts.FirstOrDefaultAsync(p => p.Id == req.PayoutId, ct);

        if (payout is null)
            throw new BusinessException("Payout not found", "PAYOUT_NOT_FOUND");

        payout.Approve(Guid.Empty); // TODO: Get admin user ID from claims
        await db.SaveChangesAsync(ct);

        logger.LogInformation("Payout {PayoutId} approved", req.PayoutId);

        return new PayoutActionResponse(req.PayoutId, (int)payout.Status);
    }
}

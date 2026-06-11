using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.PaymentService.Domain;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Features.Admin.MarkPayoutCompleted;

public sealed class MarkPayoutCompletedHandler(PaymentDbContext db)
    : ICommandHandler<Request, Response>
{
    public async ValueTask<Response> Handle(Request req, CancellationToken ct)
    {
        var payout = await db.Payouts.FirstOrDefaultAsync(p => p.Id == req.Id, ct)
            ?? throw new NotFoundException("Payout not found.");

        payout.MarkCompleted();
        await db.SaveChangesAsync(ct);

        return new Response(payout.Id, payout.Status, payout.CompletedAt);
    }
}

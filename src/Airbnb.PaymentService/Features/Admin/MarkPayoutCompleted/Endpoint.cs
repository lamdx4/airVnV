using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Airbnb.PaymentService.Domain;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Features.Admin.MarkPayoutCompleted;

public record Request
{
    public Guid Id { get; init; }
}

public record Response(Guid Id, PayoutStatus Status, DateTimeOffset? CompletedAt);

public class Endpoint(PaymentDbContext db) : Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Post("/api/admin/payouts/{id}/mark-completed");
        AllowAnonymous();
        Summary(s => s.Summary = "Admin: mark a payout as completed (bank transfer confirmed)");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var payout = await db.Payouts.FirstOrDefaultAsync(p => p.Id == req.Id, ct);
        if (payout is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        payout.MarkCompleted();
        await db.SaveChangesAsync(ct);

        var response = new Response(payout.Id, payout.Status, payout.CompletedAt);
        await Send.ResponseAsync(ApiResponse<Response>.SuccessResult(response, "Payout marked as completed"), cancellation: ct);
    }
}

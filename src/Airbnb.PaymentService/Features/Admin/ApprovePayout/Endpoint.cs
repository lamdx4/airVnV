using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Airbnb.PaymentService.Domain;
using Airbnb.ServiceDefaults.Infrastructure;
using System.Security.Claims;

namespace Airbnb.PaymentService.Features.Admin.ApprovePayout;

public record Request
{
    public Guid Id { get; init; }
}

public record Response(Guid Id, PayoutStatus Status, DateTimeOffset? ApprovedAt);

public class Endpoint(PaymentDbContext db) : Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Post("/api/admin/payouts/{id}/approve");
        AllowAnonymous();
        Summary(s => s.Summary = "Admin: approve a pending payout");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var payout = await db.Payouts.FirstOrDefaultAsync(p => p.Id == req.Id, ct);
        if (payout is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var adminId);
        payout.Approve(adminId == Guid.Empty ? Guid.CreateVersion7() : adminId);
        await db.SaveChangesAsync(ct);

        var response = new Response(payout.Id, payout.Status, payout.ApprovedAt);
        await Send.ResponseAsync(ApiResponse<Response>.SuccessResult(response, "Payout approved"), cancellation: ct);
    }
}

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

public class Endpoint(PaymentDbContext db, ILogger<Endpoint> logger)
    : Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Post("/api/admin/payouts/{id}/approve");
        AllowAnonymous();
        Summary(s => s.Summary = "Admin: approve a pending payout. Releases pending → available, then deducts payout amount from available.");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var payout = await db.Payouts
            .Include(p => p.Items)
            .FirstOrDefaultAsync(p => p.Id == req.Id, ct);
        if (payout is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var adminId);
        payout.Approve(adminId == Guid.Empty ? Guid.CreateVersion7() : adminId);

        // Get the host's balance for this currency.
        var balance = await db.HostBalances
            .FirstOrDefaultAsync(b => b.HostId == payout.HostId && b.Currency == payout.Currency, ct)
            ?? throw new BusinessException(
                $"No balance ledger found for host {payout.HostId} / {payout.Currency}. " +
                "PaymentSucceededEvent consumer may not have run.",
                "PAYOUT_NO_LEDGER");

        // For each payout item, release Pending → Available (BookingCheckedOut entry)
        // if not yet released. Idempotent: skipped when entry for (PaymentId, BookingCheckedOut) exists.
        var alreadyReleased = await db.BalanceEntries
            .Where(e => e.Type == BalanceEntryType.BookingCheckedOut
                     && payout.Items.Select(i => i.PaymentId).Contains(e.PaymentId!.Value))
            .Select(e => e.PaymentId!.Value)
            .ToListAsync(ct);

        var receivedEntries = await db.BalanceEntries
            .Where(e => e.Type == BalanceEntryType.PaymentReceived
                     && payout.Items.Select(i => i.PaymentId).Contains(e.PaymentId!.Value))
            .ToListAsync(ct);

        foreach (var item in payout.Items)
        {
            if (alreadyReleased.Contains(item.PaymentId)) continue;

            var received = receivedEntries.FirstOrDefault(e => e.PaymentId == item.PaymentId);
            if (received is null)
            {
                logger.LogWarning("No PaymentReceived entry for Payment {PaymentId} in payout {PayoutId}; skipping release.",
                    item.PaymentId, payout.Id);
                continue;
            }

            var releaseEntry = BalanceEntry.BookingCheckedOut(
                payout.HostId, received.PendingDelta, payout.Currency, item.PaymentId, item.BookingId);
            db.BalanceEntries.Add(releaseEntry);
            balance.ApplyEntry(releaseEntry);
        }

        // Now deduct the payout amount from Available (PayoutApproved entry). Idempotent guard.
        var alreadyDeducted = await db.BalanceEntries
            .AnyAsync(e => e.PayoutId == payout.Id && e.Type == BalanceEntryType.PayoutApproved, ct);
        if (!alreadyDeducted)
        {
            var deductEntry = BalanceEntry.PayoutApproved(
                payout.HostId, payout.PayoutAmount, payout.Currency, payout.Id);
            db.BalanceEntries.Add(deductEntry);
            balance.ApplyEntry(deductEntry);
        }

        await db.SaveChangesAsync(ct);

        var response = new Response(payout.Id, payout.Status, payout.ApprovedAt);
        await Send.ResponseAsync(ApiResponse<Response>.SuccessResult(response, "Payout approved and ledger updated"), cancellation: ct);
    }
}

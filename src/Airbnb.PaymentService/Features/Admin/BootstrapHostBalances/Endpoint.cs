using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Airbnb.PaymentService.Domain;
using Airbnb.PaymentService.Infrastructure.HttpClients;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Features.Admin.BootstrapHostBalances;

public record Response(
    int EntriesCreated,
    int BalancesUpdated,
    int PayoutsRebuilt,
    string Message
);

/// <summary>
/// Demo/dev tool: rebuilds Payouts, PayoutItems, HostBalances, BalanceEntries
/// from existing Payment records, using REAL host Ids fetched from UserService.
/// Wipes existing payout/ledger data first.
/// </summary>
public class Endpoint(PaymentDbContext db, UserServiceClient userClient)
    : EndpointWithoutRequest<ApiResponse<Response>>
{
    public override void Configure()
    {
        Get("/api/admin/host-balances/bootstrap"); // GET so easy to trigger from browser too
        Post("/api/admin/host-balances/bootstrap");
        AllowAnonymous();
        Summary(s => s.Summary = "Admin (dev tool): wipe + rebuild payouts + escrow ledger from existing payments");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        // 1) Fetch up to 3 real users with role 'User' to serve as fake hosts
        var fakeHosts = await GetRealHostIdsAsync(ct);
        if (fakeHosts.Count == 0)
        {
            await Send.ResponseAsync(
                ApiResponse<Response>.SuccessResult(
                    new Response(0, 0, 0, "No users found in UserService — cannot bootstrap."),
                    "No users available"),
                cancellation: ct);
            return;
        }

        // 2) Wipe existing
        await db.PayoutItems.ExecuteDeleteAsync(ct);
        await db.Payouts.ExecuteDeleteAsync(ct);
        await db.BalanceEntries.ExecuteDeleteAsync(ct);
        await db.HostBalances.ExecuteDeleteAsync(ct);

        const decimal feePercent = 0.10m;

        // 3) Load all Success payments
        var successPayments = await db.Payments.AsNoTracking()
            .Where(p => p.Status == PaymentStatus.Success)
            .OrderBy(p => p.CreatedAt)
            .Select(p => new { p.Id, p.BookingId, p.Amount, p.Currency, p.CreatedAt })
            .ToListAsync(ct);

        // 4) Round-robin assign payments to fake hosts
        var assignments = successPayments
            .Select((p, idx) => new {
                Payment = p,
                HostId = fakeHosts[idx % fakeHosts.Count],
                Fee = Math.Round(p.Amount * feePercent, 2),
                HostPortion = p.Amount - Math.Round(p.Amount * feePercent, 2),
            })
            .ToList();

        // 5) Create Payouts (one per host+currency)
        var payouts = new List<Payout>();
        var payoutItems = new List<PayoutItem>();

        var random = new Random(42);
        foreach (var group in assignments.GroupBy(a => new { a.HostId, a.Payment.Currency }))
        {
            var items = group.Select(a => new PayoutItem(
                a.Payment.BookingId,
                a.Payment.Id,
                a.Payment.Amount,
                a.Fee,
                checkIn: DateOnly.FromDateTime(a.Payment.CreatedAt.AddDays(-10).UtcDateTime),
                checkOut: DateOnly.FromDateTime(a.Payment.CreatedAt.AddDays(-7).UtcDateTime),
                propertyTitle: $"Cozy stay #{random.Next(1, 8)}",
                guestName: $"Guest {random.Next(1, 12)}"
            )).ToList();

            var payout = Payout.Create(group.Key.HostId, group.Key.Currency, items);
            payouts.Add(payout);
            payoutItems.AddRange(items);
        }

        // 6) Build BalanceEntries
        var entries = new List<BalanceEntry>();
        foreach (var a in assignments)
        {
            entries.Add(BalanceEntry.PaymentReceived(
                a.HostId, a.HostPortion, a.Payment.Currency, a.Payment.Id, a.Payment.BookingId));
            entries.Add(BalanceEntry.BookingCheckedOut(
                a.HostId, a.HostPortion, a.Payment.Currency, a.Payment.Id, a.Payment.BookingId));
        }

        // 7) Build HostBalance snapshots
        var balances = new List<HostBalance>();
        foreach (var g in entries.GroupBy(e => new { e.HostId, e.Currency }))
        {
            var balance = HostBalance.Create(g.Key.HostId, g.Key.Currency);
            balance.Recompute(g.ToList());
            balances.Add(balance);
        }

        db.Payouts.AddRange(payouts);
        db.PayoutItems.AddRange(payoutItems);
        db.BalanceEntries.AddRange(entries);
        db.HostBalances.AddRange(balances);
        await db.SaveChangesAsync(ct);

        var response = new Response(
            entries.Count,
            balances.Count,
            payouts.Count,
            $"Rebuilt: {payouts.Count} payouts, {entries.Count} ledger entries across {balances.Count} host wallets (using {fakeHosts.Count} real hosts)."
        );

        await Send.ResponseAsync(ApiResponse<Response>.SuccessResult(response, response.Message), cancellation: ct);
    }

    private async Task<List<Guid>> GetRealHostIdsAsync(CancellationToken ct)
        => await userClient.GetSampleHostIdsAsync(3, ct);
}

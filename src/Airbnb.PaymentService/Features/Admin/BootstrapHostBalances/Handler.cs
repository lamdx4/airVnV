using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.PaymentService.Domain;
using Airbnb.PaymentService.Infrastructure.HttpClients;

namespace Airbnb.PaymentService.Features.Admin.BootstrapHostBalances;

// Demo/dev tool handler: wipes + rebuilds Payouts/PayoutItems/HostBalances/BalanceEntries
// from existing Payment records. Each payment is credited to the actual host of the booking
// it belongs to (resolved via BookingService).
public sealed class BootstrapHostBalancesHandler(PaymentDbContext db, BookingServiceClient bookingClient)
    : ICommandHandler<Request, Response>
{
    private const decimal FeePercent = 0.10m;

    public async ValueTask<Response> Handle(Request req, CancellationToken ct)
    {
        var successPayments = await db.Payments.AsNoTracking()
            .Where(p => p.Status == PaymentStatus.Success)
            .OrderBy(p => p.CreatedAt)
            .Select(p => new { p.Id, p.BookingId, p.Amount, p.Currency, p.CreatedAt })
            .ToListAsync(ct);

        if (successPayments.Count == 0)
            return new Response(0, 0, 0, "No Success payments to bootstrap.");

        var bookingInfos = await bookingClient.GetBasicInfosAsync(
            successPayments.Select(p => p.BookingId), ct);

        var assignments = successPayments
            .Where(p => bookingInfos.ContainsKey(p.BookingId))
            .Select(p => new {
                Payment = p,
                HostId = bookingInfos[p.BookingId].HostId,
                Fee = Math.Round(p.Amount * FeePercent, 2),
                HostPortion = p.Amount - Math.Round(p.Amount * FeePercent, 2),
            })
            .ToList();

        if (assignments.Count == 0)
            return new Response(0, 0, 0,
                $"Could not resolve any host from {successPayments.Count} payments — BookingService unavailable?");

        await db.PayoutItems.ExecuteDeleteAsync(ct);
        await db.Payouts.ExecuteDeleteAsync(ct);
        await db.BalanceEntries.ExecuteDeleteAsync(ct);
        await db.HostBalances.ExecuteDeleteAsync(ct);

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

        var entries = new List<BalanceEntry>();
        foreach (var a in assignments)
        {
            entries.Add(BalanceEntry.PaymentReceived(
                a.HostId, a.HostPortion, a.Payment.Currency, a.Payment.Id, a.Payment.BookingId));
            entries.Add(BalanceEntry.BookingCheckedOut(
                a.HostId, a.HostPortion, a.Payment.Currency, a.Payment.Id, a.Payment.BookingId));
        }

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

        var skipped = successPayments.Count - assignments.Count;
        return new Response(
            entries.Count,
            balances.Count,
            payouts.Count,
            $"Rebuilt: {payouts.Count} payouts, {entries.Count} ledger entries across {balances.Count} host wallets (skipped {skipped} payments with unresolved host)."
        );
    }
}

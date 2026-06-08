using Airbnb.PaymentService.Domain;
using Airbnb.PaymentService.Infrastructure;
using Airbnb.PaymentService.Infrastructure.HttpClients;
using Airbnb.SharedKernel.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.PaymentService.Features.Consumers;

/// <summary>
/// Listens to its own PaymentSucceededEvent (after outbox publishes) and writes
/// a PaymentReceived ledger entry + updates the host's PendingBalance.
///
/// Why self-consume instead of inline in Handler?
///   - Keeps the payment confirmation transaction small and fast.
///   - Lets ledger sync retry independently if BookingService/UserService is slow.
///   - Same idempotency story as other consumers (check by PaymentId+Type).
/// </summary>
public class PaymentSucceededLedgerConsumer(
    PaymentDbContext db,
    BookingServiceClient bookingClient,
    ILogger<PaymentSucceededLedgerConsumer> logger)
    : IConsumer<PaymentSucceededEvent>
{
    public async Task Consume(ConsumeContext<PaymentSucceededEvent> ctx)
    {
        var msg = ctx.Message;

        // 1) Idempotency: skip if PaymentReceived entry already exists for this payment.
        var exists = await db.BalanceEntries
            .AnyAsync(e => e.PaymentId == msg.PaymentId
                        && e.Type == BalanceEntryType.PaymentReceived,
                      ctx.CancellationToken);
        if (exists)
        {
            logger.LogInformation("Ledger entry for Payment {PaymentId} already exists, skipping.", msg.PaymentId);
            return;
        }

        // 2) Resolve HostId via BookingService.
        var booking = await bookingClient.GetBookingBasicInfoAsync(msg.BookingId, ctx.CancellationToken);
        if (booking is null)
        {
            logger.LogWarning("Booking {BookingId} not found while writing ledger for Payment {PaymentId}.",
                msg.BookingId, msg.PaymentId);
            return;
        }

        // 3) Compute host portion using current PlatformSettings.
        var setting = await db.PlatformSettings.FirstOrDefaultAsync(ctx.CancellationToken)
                      ?? PlatformSetting.CreateDefault();
        var fee = Math.Round(msg.Amount * setting.PlatformFeePercent / 100m, 2);
        var hostPortion = msg.Amount - fee;

        // 4) Get or create the host's balance snapshot for this currency.
        var balance = await db.HostBalances
            .FirstOrDefaultAsync(b => b.HostId == booking.HostId && b.Currency == msg.Currency,
                                 ctx.CancellationToken);
        var isNewBalance = balance is null;
        balance ??= HostBalance.Create(booking.HostId, msg.Currency);

        // 5) Write ledger + apply to snapshot in one transaction.
        var entry = BalanceEntry.PaymentReceived(
            booking.HostId, hostPortion, msg.Currency, msg.PaymentId, msg.BookingId);

        db.BalanceEntries.Add(entry);
        balance.ApplyEntry(entry);
        if (isNewBalance) db.HostBalances.Add(balance);

        await db.SaveChangesAsync(ctx.CancellationToken);

        logger.LogInformation(
            "Ledger updated: Host {HostId} Pending += {Amount} {Currency} (Payment {PaymentId}).",
            booking.HostId, hostPortion, msg.Currency, msg.PaymentId);
    }
}

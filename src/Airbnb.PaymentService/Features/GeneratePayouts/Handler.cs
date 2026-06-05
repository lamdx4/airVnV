using Airbnb.PaymentService.Domain;
using Airbnb.PaymentService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.PaymentService.Features.GeneratePayouts;

public sealed class Handler(PaymentDbContext db, ILogger<Handler> logger)
    : ICommandHandler<Command, GeneratePayoutsResponse>
{
    public async ValueTask<GeneratePayoutsResponse> Handle(Command req, CancellationToken ct)
    {
        // Find bookings that have a successful payment but are not yet included in any payout item
        var successfulPayments = await db.Payments
            .AsNoTracking()
            .Where(p => p.Status == PaymentStatus.Success)
            .Select(p => new { p.Id, p.BookingId, p.Amount, p.Currency })
            .ToListAsync(ct);

        if (successfulPayments.Count == 0)
            return new GeneratePayoutsResponse(0, 0);

        // Get booking IDs that are already in a payout item (non-cancelled payouts)
        var bookedBookingIds = await db.PayoutItems
            .AsNoTracking()
            .Join(db.Payouts, pi => pi.PayoutId, p => p.Id, (pi, p) => new { pi.BookingId, PayoutStatus = p.Status })
            .Where(x => x.PayoutStatus != PayoutStatus.Cancelled)
            .Select(x => x.BookingId)
            .Distinct()
            .ToHashSetAsync(ct);

        // Filter to payments whose bookings are not yet paid out
        // NOTE: In a full implementation, we'd also verify the booking is CheckedOut
        // by calling BookingService. For now, we use successful payments as the eligibility criteria.
        var eligiblePayments = successfulPayments
            .Where(p => !bookedBookingIds.Contains(p.BookingId))
            .ToList();

        if (eligiblePayments.Count == 0)
            return new GeneratePayoutsResponse(0, 0);

        // Group by Host (we need to resolve HostId from BookingService — stub for now)
        // In full implementation: fetch booking details from BookingService to get HostId, CheckIn, CheckOut, PropertyTitle, GuestName, ServiceFee
        // For now, we create payouts per booking as individual payouts (1 item each)
        // This is a simplified version; the real version would group by HostId

        var payoutsGenerated = 0;
        var bookingsProcessed = 0;

        // Group eligible payments by currency
        foreach (var group in eligiblePayments.GroupBy(p => p.Currency))
        {
            // TODO: In full implementation, group by HostId within each currency
            // For now, create one payout per eligible payment as a stub
            foreach (var payment in group)
            {
                var serviceFee = payment.Amount * 0.10m; // TODO: Use active PlatformFeeConfig
                var hostEarning = payment.Amount - serviceFee;

                var item = PayoutItem.Create(
                    payment.BookingId,
                    payment.Id,
                    payment.Amount,
                    serviceFee,
                    default, // CheckIn — needs BookingService data
                    default, // CheckOut — needs BookingService data
                    "Property", // PropertyTitle — needs BookingService data
                    "Guest" // GuestName — needs BookingService data
                );

                // Stub: use a placeholder HostId
                var payout = Payout.Create(Guid.Empty, group.Key, [item]);
                db.Payouts.Add(payout);
                payoutsGenerated++;
                bookingsProcessed++;
            }
        }

        await db.SaveChangesAsync(ct);

        logger.LogInformation("Generated {Count} payouts for {Bookings} bookings", payoutsGenerated, bookingsProcessed);

        return new GeneratePayoutsResponse(payoutsGenerated, bookingsProcessed);
    }
}

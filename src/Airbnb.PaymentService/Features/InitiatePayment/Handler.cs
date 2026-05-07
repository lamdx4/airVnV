using Airbnb.PaymentService.Domain;
using Airbnb.PaymentService.Infrastructure;
using Airbnb.PaymentService.Infrastructure.HttpClients;
using Airbnb.PaymentService.Infrastructure.PaymentGateways;
using Airbnb.ServiceDefaults.Infrastructure;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.PaymentService.Features.InitiatePayment;

public sealed class Handler(
    PaymentDbContext db,
    BookingServiceClient bookingServiceClient,
    PaymentProviderResolver providerResolver,
    ILogger<Handler> logger)
    : ICommandHandler<Request, Response>
{
    public async ValueTask<Response> Handle(Request req, CancellationToken ct)
    {
        // 1. Idempotency Guard (Check for existing pending payment)
        var existing = await db.Payments
            .FirstOrDefaultAsync(p => p.BookingId == req.BookingId && p.Status == PaymentStatus.Pending, ct);

        if (existing != null)
        {
            if (existing.IsStillValid())
            {
                logger.LogInformation("Reusing existing valid payment URL for Booking {BookingId}", req.BookingId);
                return new Response(existing.PaymentUrl!);
            }
            
            // Mark as expired if it's too old
            existing.MarkAsExpired();
            await db.SaveChangesAsync(ct);
            logger.LogInformation("Existing payment for Booking {BookingId} expired, creating new one", req.BookingId);
        }

        // 2. Fetch Booking Info & Security Check
        var booking = await bookingServiceClient.GetBookingBasicInfoAsync(req.BookingId, ct);
        if (booking == null)
            throw new BusinessException("Booking not found", "PAYMENT_BOOKING_NOT_FOUND");

        if (booking.GuestId != req.UserId)
        {
            logger.LogWarning("User {UserId} attempted to pay for Booking {BookingId} owned by {GuestId}", 
                req.UserId, req.BookingId, booking.GuestId);
            throw new BusinessException("You are not authorized to pay for this booking", "PAYMENT_FORBIDDEN");
        }

        // 3. Resolve Provider
        var provider = await providerResolver.ResolveAsync(booking.CountryCode, ct);

        // 4. Create Payment Record with Race Condition Handling
        var payment = Payment.Create(req.BookingId, booking.TotalPrice, booking.CurrencyCode);
        db.Payments.Add(payment);

        try 
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException)
        {
            // Race condition: someone else created a pending payment just now
            logger.LogInformation("Race condition detected for Booking {BookingId}, retrying idempotency check", req.BookingId);
            var freshExisting = await db.Payments
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.BookingId == req.BookingId && p.Status == PaymentStatus.Pending, ct);
            
            if (freshExisting != null && freshExisting.IsStillValid())
                return new Response(freshExisting.PaymentUrl!);
                
            throw new BusinessException("Payment is already being processed. Please refresh.", "PAYMENT_CONCURRENCY_ERROR");
        }

        // 5. Generate Provider URL
        var ipAddress = req.IpAddress ?? "127.0.0.1";
        var result = await provider.GeneratePaymentUrlAsync(payment, ipAddress, ct);
        
        // 6. Update Payment with URL and Expiry
        payment.Initiate(result.Url, result.ExpiresAt);
        await db.SaveChangesAsync(ct);

        logger.LogInformation("Initiated {Provider} payment for Booking {BookingId}", provider.ProviderName, req.BookingId);
        
        return new Response(result.Url);
    }
}

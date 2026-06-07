using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Airbnb.PaymentService.Domain;
using Airbnb.PaymentService.Infrastructure.HttpClients;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Features.Admin.GetAdminPaymentDetail;

public record Request
{
    public Guid Id { get; init; }
}

public record Response(
    Guid Id,
    Guid BookingId,
    Guid? GuestId,
    string? GuestName,
    string? GuestEmail,
    string? GuestAvatarUrl,
    decimal Amount,
    string Currency,
    PaymentStatus Status,
    string? TransactionId,
    string? PaymentUrl,
    DateTimeOffset CreatedAt,
    DateTimeOffset? ExpiresAt
);

public class Endpoint(PaymentDbContext db, BookingServiceClient bookingClient, UserServiceClient userClient)
    : Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Get("/api/admin/payments/{id}");
        AllowAnonymous();
        Summary(s => s.Summary = "Admin: get a single pay-in transaction");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var p = await db.Payments.AsNoTracking().FirstOrDefaultAsync(x => x.Id == req.Id, ct);
        if (p is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        Guid? guestId = null;
        string? guestName = null, guestEmail = null, guestAvatarUrl = null;

        var booking = await bookingClient.GetBookingBasicInfoAsync(p.BookingId, ct);
        if (booking is not null)
        {
            guestId = booking.GuestId;
            var users = await userClient.GetBasicInfosAsync(new[] { booking.GuestId }, ct);
            if (users.TryGetValue(booking.GuestId, out var u))
            {
                guestName = u.FullName;
                guestEmail = u.Email;
                guestAvatarUrl = u.AvatarUrl;
            }
        }

        var response = new Response(
            p.Id, p.BookingId, guestId, guestName, guestEmail, guestAvatarUrl,
            p.Amount, p.Currency, p.Status,
            p.TransactionId, p.PaymentUrl, p.CreatedAt, p.ExpiresAt
        );
        await Send.ResponseAsync(ApiResponse<Response>.SuccessResult(response), cancellation: ct);
    }
}

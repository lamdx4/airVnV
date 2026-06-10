using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.PaymentService.Domain;
using Airbnb.PaymentService.Infrastructure.HttpClients;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Features.Admin.GetAdminPaymentDetail;

public sealed class GetAdminPaymentDetailHandler(
    PaymentDbContext db,
    BookingServiceClient bookingClient,
    UserServiceClient userClient)
    : IQueryHandler<Request, Response>
{
    public async ValueTask<Response> Handle(Request req, CancellationToken ct)
    {
        var p = await db.Payments.AsNoTracking().FirstOrDefaultAsync(x => x.Id == req.Id, ct)
            ?? throw new NotFoundException("Payment not found.");

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

        return new Response(
            p.Id, p.BookingId, guestId, guestName, guestEmail, guestAvatarUrl,
            p.Amount, p.Currency, p.Status,
            p.TransactionId, p.PaymentUrl, p.CreatedAt, p.ExpiresAt
        );
    }
}

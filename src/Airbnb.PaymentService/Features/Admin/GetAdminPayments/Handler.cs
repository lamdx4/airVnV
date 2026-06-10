using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.PaymentService.Domain;
using Airbnb.PaymentService.Infrastructure.HttpClients;

namespace Airbnb.PaymentService.Features.Admin.GetAdminPayments;

public sealed class GetAdminPaymentsHandler(
    PaymentDbContext db,
    BookingServiceClient bookingClient,
    UserServiceClient userClient)
    : IQueryHandler<Request, PagedResponse<AdminPaymentItem>>
{
    public async ValueTask<PagedResponse<AdminPaymentItem>> Handle(Request req, CancellationToken ct)
    {
        var page = req.Page < 1 ? 1 : req.Page;
        var pageSize = req.PageSize is < 1 or > 100 ? 20 : req.PageSize;

        var query = new PaymentQueryBuilder(db.Payments.AsNoTracking())
            .WithStatus(req.Status)
            .WithSearch(req.Search)
            .WithDateRange(req.From, req.To)
            .OrderByCreatedAt(req.SortOrder)
            .Build();

        var total = await query.CountAsync(ct);

        var rows = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new {
                p.Id, p.BookingId, p.Amount, p.Currency, p.Status,
                p.TransactionId, p.CreatedAt, p.ExpiresAt
            })
            .ToListAsync(ct);

        var bookingInfos = await bookingClient.GetBasicInfosAsync(rows.Select(r => r.BookingId), ct);
        var guestIds = bookingInfos.Values.Select(b => b.GuestId).Distinct().ToList();
        var guestInfos = await userClient.GetBasicInfosAsync(guestIds, ct);

        var items = rows.Select(p =>
        {
            Guid? guestId = null;
            string? guestName = null, guestEmail = null, guestAvatarUrl = null;
            if (bookingInfos.TryGetValue(p.BookingId, out var b))
            {
                guestId = b.GuestId;
                if (guestInfos.TryGetValue(b.GuestId, out var u))
                {
                    guestName = u.FullName;
                    guestEmail = u.Email;
                    guestAvatarUrl = u.AvatarUrl;
                }
            }
            return new AdminPaymentItem(
                p.Id, p.BookingId, guestId, guestName, guestEmail, guestAvatarUrl,
                p.Amount, p.Currency, p.Status, p.TransactionId, p.CreatedAt, p.ExpiresAt
            );
        }).ToList();

        return new PagedResponse<AdminPaymentItem>(items, total, page, pageSize);
    }
}

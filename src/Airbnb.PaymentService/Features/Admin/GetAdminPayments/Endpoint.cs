using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Airbnb.PaymentService.Domain;
using Airbnb.PaymentService.Infrastructure.HttpClients;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Features.Admin.GetAdminPayments;

public record Request
{
    [BindFrom("page")] public int Page { get; init; } = 1;
    [BindFrom("pageSize")] public int PageSize { get; init; } = 20;
    public string? Status { get; init; }
    public string? Search { get; init; } // matches BookingId / TransactionId substring
    public string? From { get; init; }   // yyyy-MM-dd
    public string? To { get; init; }     // yyyy-MM-dd
    public string? SortOrder { get; init; } = "desc"; // by CreatedAt
}

public record AdminPaymentItem(
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
    DateTimeOffset CreatedAt,
    DateTimeOffset? ExpiresAt
);

public record PagedResponse<T>(
    List<T> Items,
    int TotalCount,
    int Page,
    int PageSize
)
{
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;
}

public class Endpoint(PaymentDbContext db, BookingServiceClient bookingClient, UserServiceClient userClient) : Endpoint<Request, ApiResponse<PagedResponse<AdminPaymentItem>>>
{
    public override void Configure()
    {
        Get("/api/admin/payments");
        AllowAnonymous();
        Summary(s => s.Summary = "Admin: list pay-in transactions with filters & pagination");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var page = req.Page < 1 ? 1 : req.Page;
        var pageSize = req.PageSize is < 1 or > 100 ? 20 : req.PageSize;

        // Builder Pattern: handler không còn chứa logic xây query —
        // toàn bộ filter + sort gói gọn trong PaymentQueryBuilder.
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

        var response = new PagedResponse<AdminPaymentItem>(items, total, page, pageSize);
        await Send.ResponseAsync(ApiResponse<PagedResponse<AdminPaymentItem>>.SuccessResult(response), cancellation: ct);
    }
}

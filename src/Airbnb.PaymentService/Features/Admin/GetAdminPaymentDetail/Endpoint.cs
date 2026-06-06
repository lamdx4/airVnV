using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Airbnb.PaymentService.Domain;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Features.Admin.GetAdminPaymentDetail;

public record Request
{
    public Guid Id { get; init; }
}

public record Response(
    Guid Id,
    Guid BookingId,
    decimal Amount,
    string Currency,
    PaymentStatus Status,
    string? TransactionId,
    string? PaymentUrl,
    DateTimeOffset CreatedAt,
    DateTimeOffset? ExpiresAt
);

public class Endpoint(PaymentDbContext db) : Endpoint<Request, ApiResponse<Response>>
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

        var response = new Response(
            p.Id, p.BookingId, p.Amount, p.Currency, p.Status,
            p.TransactionId, p.PaymentUrl, p.CreatedAt, p.ExpiresAt
        );
        await Send.ResponseAsync(ApiResponse<Response>.SuccessResult(response), cancellation: ct);
    }
}

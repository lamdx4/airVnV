using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Airbnb.PaymentService.Domain;
using Airbnb.PaymentService.Infrastructure.HttpClients;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Features.Admin.GetAdminPayoutDetail;

public record Request
{
    public Guid Id { get; init; }
}

public record PayoutItemDto(
    Guid Id,
    Guid BookingId,
    Guid PaymentId,
    decimal BookingTotal,
    decimal ServiceFee,
    decimal HostEarning,
    DateOnly CheckIn,
    DateOnly CheckOut,
    string PropertyTitle,
    string GuestName
);

public record Response(
    Guid Id,
    Guid HostId,
    string? HostName,
    string? HostEmail,
    string? HostAvatarUrl,
    decimal TotalEarnings,
    decimal PlatformFee,
    decimal PayoutAmount,
    string Currency,
    PayoutStatus Status,
    int ItemCount,
    DateTimeOffset CreatedAt,
    Guid? ApprovedBy,
    DateTimeOffset? ApprovedAt,
    DateTimeOffset? CompletedAt,
    List<PayoutItemDto> Items
);

public class Endpoint(PaymentDbContext db, UserServiceClient userClient) : Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Get("/api/admin/payouts/{id}");
        AllowAnonymous();
        Summary(s => s.Summary = "Admin: get payout detail with line items");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var payout = await db.Payouts.AsNoTracking()
            .Include(p => p.Items)
            .FirstOrDefaultAsync(p => p.Id == req.Id, ct);

        if (payout is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var hostInfos = await userClient.GetBasicInfosAsync(new[] { payout.HostId }, ct);
        hostInfos.TryGetValue(payout.HostId, out var u);

        var response = new Response(
            payout.Id,
            payout.HostId,
            u?.FullName, u?.Email, u?.AvatarUrl,
            payout.TotalEarnings,
            payout.PlatformFee,
            payout.PayoutAmount,
            payout.Currency,
            payout.Status,
            payout.ItemCount,
            payout.CreatedAt,
            payout.ApprovedBy,
            payout.ApprovedAt,
            payout.CompletedAt,
            payout.Items
                .OrderBy(i => i.CheckOut)
                .Select(i => new PayoutItemDto(
                    i.Id, i.BookingId, i.PaymentId,
                    i.BookingTotal, i.ServiceFee, i.HostEarning,
                    i.CheckIn, i.CheckOut, i.PropertyTitle, i.GuestName
                )).ToList()
        );

        await Send.ResponseAsync(ApiResponse<Response>.SuccessResult(response), cancellation: ct);
    }
}

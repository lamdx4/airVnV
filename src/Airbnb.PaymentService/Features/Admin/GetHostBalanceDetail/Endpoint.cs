using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Airbnb.PaymentService.Domain;
using Airbnb.PaymentService.Infrastructure.HttpClients;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Features.Admin.GetHostBalanceDetail;

public record Request
{
    public Guid Id { get; init; }
}

public record EntryDto(
    Guid Id,
    BalanceEntryType Type,
    decimal PendingDelta,
    decimal AvailableDelta,
    Guid? PaymentId,
    Guid? PayoutId,
    Guid? BookingId,
    string? Note,
    DateTimeOffset CreatedAt
);

public record Response(
    Guid Id,
    Guid HostId,
    string? HostName,
    string? HostEmail,
    string? HostAvatarUrl,
    string Currency,
    decimal PendingBalance,
    decimal AvailableBalance,
    DateTimeOffset UpdatedAt,
    List<EntryDto> Entries
);

public class Endpoint(PaymentDbContext db, UserServiceClient userClient) : Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Get("/api/admin/host-balances/{id}");
        AllowAnonymous();
        Summary(s => s.Summary = "Admin: host balance detail with ledger entries");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var balance = await db.HostBalances.AsNoTracking().FirstOrDefaultAsync(b => b.Id == req.Id, ct);
        if (balance is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var entries = await db.BalanceEntries.AsNoTracking()
            .Where(e => e.HostId == balance.HostId && e.Currency == balance.Currency)
            .OrderByDescending(e => e.CreatedAt)
            .Take(100)
            .Select(e => new EntryDto(
                e.Id, e.Type, e.PendingDelta, e.AvailableDelta,
                e.PaymentId, e.PayoutId, e.BookingId, e.Note, e.CreatedAt
            ))
            .ToListAsync(ct);

        var hostInfos = await userClient.GetBasicInfosAsync(new[] { balance.HostId }, ct);
        hostInfos.TryGetValue(balance.HostId, out var u);

        var response = new Response(
            balance.Id, balance.HostId,
            u?.FullName, u?.Email, u?.AvatarUrl,
            balance.Currency,
            balance.PendingBalance, balance.AvailableBalance, balance.UpdatedAt,
            entries
        );

        await Send.ResponseAsync(ApiResponse<Response>.SuccessResult(response), cancellation: ct);
    }
}

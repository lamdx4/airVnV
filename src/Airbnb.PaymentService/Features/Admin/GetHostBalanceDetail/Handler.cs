using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.PaymentService.Domain;
using Airbnb.PaymentService.Infrastructure.HttpClients;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Features.Admin.GetHostBalanceDetail;

public sealed class GetHostBalanceDetailHandler(PaymentDbContext db, UserServiceClient userClient)
    : IQueryHandler<Request, Response>
{
    public async ValueTask<Response> Handle(Request req, CancellationToken ct)
    {
        var balance = await db.HostBalances.AsNoTracking().FirstOrDefaultAsync(b => b.Id == req.Id, ct)
            ?? throw new NotFoundException("Host balance not found.");

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

        return new Response(
            balance.Id, balance.HostId,
            u?.FullName, u?.Email, u?.AvatarUrl,
            balance.Currency,
            balance.PendingBalance, balance.AvailableBalance, balance.UpdatedAt,
            entries
        );
    }
}

using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.PaymentService.Domain;
using Airbnb.PaymentService.Infrastructure.HttpClients;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Features.Admin.GetAdminPayoutDetail;

public sealed class GetAdminPayoutDetailHandler(PaymentDbContext db, UserServiceClient userClient)
    : IQueryHandler<Request, Response>
{
    public async ValueTask<Response> Handle(Request req, CancellationToken ct)
    {
        var payout = await db.Payouts.AsNoTracking()
            .Include(p => p.Items)
            .FirstOrDefaultAsync(p => p.Id == req.Id, ct)
            ?? throw new NotFoundException("Payout not found.");

        var hostInfos = await userClient.GetBasicInfosAsync(new[] { payout.HostId }, ct);
        hostInfos.TryGetValue(payout.HostId, out var u);

        return new Response(
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
    }
}

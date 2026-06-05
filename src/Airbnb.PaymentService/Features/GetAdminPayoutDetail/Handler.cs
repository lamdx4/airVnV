using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.PaymentService.Domain;
using Airbnb.PaymentService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Features.GetAdminPayoutDetail;

public sealed class Handler(PaymentDbContext db) : IQueryHandler<Request, AdminPayoutDetailResponse>
{
    public async ValueTask<AdminPayoutDetailResponse> Handle(Request req, CancellationToken ct)
    {
        var payout = await db.Payouts
            .AsNoTracking()
            .Where(p => p.Id == req.PayoutId)
            .Select(p => new
            {
                p.Id,
                p.HostId,
                p.TotalEarnings,
                p.PlatformFee,
                p.PayoutAmount,
                p.Currency,
                Status = (int)p.Status,
                p.ItemCount,
                p.ApprovedBy,
                p.ApprovedAt,
                p.CompletedAt,
                p.CreatedAt,
            })
            .FirstOrDefaultAsync(ct);

        if (payout is null)
            throw new BusinessException("Payout not found", "PAYOUT_NOT_FOUND");

        var items = await db.PayoutItems
            .AsNoTracking()
            .Where(i => i.PayoutId == req.PayoutId)
            .OrderBy(i => i.CheckIn)
            .Select(i => new PayoutItemDto(
                i.Id,
                i.BookingId,
                i.PaymentId,
                i.BookingTotal,
                i.ServiceFee,
                i.HostEarning,
                i.CheckIn,
                i.CheckOut,
                i.PropertyTitle,
                i.GuestName
            ))
            .ToListAsync(ct);

        return new AdminPayoutDetailResponse(
            payout.Id,
            payout.HostId,
            payout.TotalEarnings,
            payout.PlatformFee,
            payout.PayoutAmount,
            payout.Currency,
            payout.Status,
            payout.ItemCount,
            payout.ApprovedBy,
            payout.ApprovedAt,
            payout.CompletedAt,
            payout.CreatedAt,
            items
        );
    }
}

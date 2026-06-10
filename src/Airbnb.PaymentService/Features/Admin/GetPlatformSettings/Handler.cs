using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.PaymentService.Domain;

namespace Airbnb.PaymentService.Features.Admin.GetPlatformSettings;

public sealed class GetPlatformSettingsHandler(PaymentDbContext db)
    : IQueryHandler<Request, Response>
{
    public async ValueTask<Response> Handle(Request req, CancellationToken ct)
    {
        // Read-or-init: tạo bản default lần đầu nếu chưa tồn tại.
        var entity = await db.PlatformSettings.AsNoTracking().FirstOrDefaultAsync(ct);
        if (entity is null)
        {
            entity = PlatformSetting.CreateDefault();
            db.PlatformSettings.Add(entity);
            await db.SaveChangesAsync(ct);
        }

        return new Response(
            entity.Id,
            entity.PlatformFeePercent,
            entity.MinPayoutAmount,
            entity.DefaultCurrency,
            entity.UpdatedAt,
            entity.UpdatedBy
        );
    }
}

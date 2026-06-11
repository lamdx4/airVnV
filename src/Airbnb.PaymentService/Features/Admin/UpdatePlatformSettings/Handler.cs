using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.PaymentService.Domain;

namespace Airbnb.PaymentService.Features.Admin.UpdatePlatformSettings;

public sealed class UpdatePlatformSettingsHandler(PaymentDbContext db)
    : ICommandHandler<Request, Response>
{
    public async ValueTask<Response> Handle(Request req, CancellationToken ct)
    {
        var entity = await db.PlatformSettings.FirstOrDefaultAsync(ct);
        if (entity is null)
        {
            entity = PlatformSetting.CreateDefault();
            db.PlatformSettings.Add(entity);
        }

        entity.Update(req.PlatformFeePercent, req.MinPayoutAmount, req.DefaultCurrency, req.Actor);
        await db.SaveChangesAsync(ct);

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

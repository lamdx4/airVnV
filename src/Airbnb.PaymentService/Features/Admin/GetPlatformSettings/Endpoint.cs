using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Airbnb.PaymentService.Domain;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Features.Admin.GetPlatformSettings;

public record Response(
    Guid Id,
    decimal PlatformFeePercent,
    decimal MinPayoutAmount,
    string DefaultCurrency,
    DateTimeOffset UpdatedAt,
    string? UpdatedBy
);

public class Endpoint(PaymentDbContext db) : EndpointWithoutRequest<ApiResponse<Response>>
{
    public override void Configure()
    {
        Get("/api/admin/settings/platform");
        AllowAnonymous();
        Summary(s => s.Summary = "Admin: get current platform fee settings");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var entity = await db.PlatformSettings.AsNoTracking().FirstOrDefaultAsync(ct);
        if (entity is null)
        {
            entity = PlatformSetting.CreateDefault();
            db.PlatformSettings.Add(entity);
            await db.SaveChangesAsync(ct);
        }

        var response = new Response(
            entity.Id,
            entity.PlatformFeePercent,
            entity.MinPayoutAmount,
            entity.DefaultCurrency,
            entity.UpdatedAt,
            entity.UpdatedBy
        );

        await Send.ResponseAsync(ApiResponse<Response>.SuccessResult(response), cancellation: ct);
    }
}

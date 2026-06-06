using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Airbnb.PaymentService.Domain;
using Airbnb.ServiceDefaults.Infrastructure;
using System.Security.Claims;

namespace Airbnb.PaymentService.Features.Admin.UpdatePlatformSettings;

public record Request
{
    public decimal PlatformFeePercent { get; init; }
    public decimal MinPayoutAmount { get; init; }
    public string DefaultCurrency { get; init; } = "USD";
}

public record Response(
    Guid Id,
    decimal PlatformFeePercent,
    decimal MinPayoutAmount,
    string DefaultCurrency,
    DateTimeOffset UpdatedAt,
    string? UpdatedBy
);

public class Endpoint(PaymentDbContext db) : Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Put("/api/admin/settings/platform");
        AllowAnonymous();
        Summary(s => s.Summary = "Admin: update platform fee settings");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var entity = await db.PlatformSettings.FirstOrDefaultAsync(ct);
        if (entity is null)
        {
            entity = PlatformSetting.CreateDefault();
            db.PlatformSettings.Add(entity);
        }

        var actor = User.FindFirstValue(ClaimTypes.Email)
                    ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

        entity.Update(req.PlatformFeePercent, req.MinPayoutAmount, req.DefaultCurrency, actor);
        await db.SaveChangesAsync(ct);

        var response = new Response(
            entity.Id,
            entity.PlatformFeePercent,
            entity.MinPayoutAmount,
            entity.DefaultCurrency,
            entity.UpdatedAt,
            entity.UpdatedBy
        );

        await Send.ResponseAsync(ApiResponse<Response>.SuccessResult(response, "Settings updated"), cancellation: ct);
    }
}

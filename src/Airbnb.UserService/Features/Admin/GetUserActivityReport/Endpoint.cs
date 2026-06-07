using FastEndpoints;
using Airbnb.ServiceDefaults.Infrastructure;
using Airbnb.UserService.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.UserService.Features.Admin.GetUserActivityReport;

public class Endpoint(UserDbContext db) : EndpointWithoutRequest<ApiResponse<Response>>
{
    public override void Configure()
    {
        Get("/user-activity");
        Group<ReportsGroup>();
        Roles("Admin", "Moderator");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var d7 = now.AddDays(-7);
        var d30 = now.AddDays(-30);
        var d90 = now.AddDays(-90);

        var active7 = await db.Users.AsNoTracking()
            .CountAsync(u => u.LastLoginAt >= d7, ct);

        var active30 = await db.Users.AsNoTracking()
            .CountAsync(u => u.LastLoginAt >= d30 && u.LastLoginAt < d7, ct);

        var active90 = await db.Users.AsNoTracking()
            .CountAsync(u => u.LastLoginAt >= d90 && u.LastLoginAt < d30, ct);

        var inactive = await db.Users.AsNoTracking()
            .CountAsync(u => u.LastLoginAt != null && u.LastLoginAt < d90, ct);

        var never = await db.Users.AsNoTracking()
            .CountAsync(u => u.LastLoginAt == null, ct);

        Response = ApiResponse<Response>.SuccessResult(
            new Response(active7, active30, active90, inactive, never),
            "User activity retrieved");
    }
}

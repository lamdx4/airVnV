using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.UserService.Infrastructure;

namespace Airbnb.UserService.Features.Admin.GetUserActivityReport;

public sealed class GetUserActivityReportHandler(UserDbContext db)
    : IQueryHandler<Request, Response>
{
    public async ValueTask<Response> Handle(Request req, CancellationToken ct)
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

        return new Response(active7, active30, active90, inactive, never);
    }
}

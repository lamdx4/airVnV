using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.UserService.Domain;
using Airbnb.UserService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Admin.GetReportSummary;

public sealed class GetReportSummaryHandler(UserDbContext db)
    : IQueryHandler<Request, Response>
{
    public async ValueTask<Response> Handle(Request req, CancellationToken ct)
    {
        if (!DateOnly.TryParse(req.From, out var fromDate) || !DateOnly.TryParse(req.To, out var toDate))
            throw new BusinessException("Invalid date range. Use yyyy-MM-dd.", "INVALID_DATE_RANGE");

        var fromStart = fromDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var toEnd = toDate.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);

        var totalUsers = await db.Users.AsNoTracking().CountAsync(ct);

        var newUsers = await db.Users.AsNoTracking()
            .Where(u => u.CreatedAt >= fromStart && u.CreatedAt <= toEnd)
            .CountAsync(ct);

        var activeUsers = await db.Users.AsNoTracking()
            .Where(u => u.Status == UserStatus.Active).CountAsync(ct);

        var suspendedUsers = await db.Users.AsNoTracking()
            .Where(u => u.Status == UserStatus.Suspended).CountAsync(ct);

        var bannedUsers = await db.Users.AsNoTracking()
            .Where(u => u.Status == UserStatus.Banned).CountAsync(ct);

        var userCount = await db.Users.AsNoTracking()
            .Where(u => u.Role == UserRole.User).CountAsync(ct);

        var adminCount = await db.Users.AsNoTracking()
            .Where(u => u.Role == UserRole.Admin).CountAsync(ct);

        return new Response(
            TotalRevenue: 0,
            TotalBookings: 0,
            AverageBookingValue: 0,
            OccupancyRate: 0,
            NewUsers: newUsers,
            NewProperties: 0,
            TotalUsers: totalUsers,
            ActiveUsers: activeUsers,
            SuspendedUsers: suspendedUsers,
            BannedUsers: bannedUsers,
            UserCount: userCount,
            AdminCount: adminCount
        );
    }
}

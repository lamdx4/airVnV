using FastEndpoints;
using Airbnb.ServiceDefaults.Infrastructure;
using Airbnb.UserService.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Airbnb.UserService.Domain;

namespace Airbnb.UserService.Features.Admin.GetReportSummary;

public class Endpoint(UserDbContext db) : Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Get("/summary");
        Group<ReportsGroup>();
        Roles("Admin", "Moderator");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var fromDate = DateOnly.Parse(req.From);
        var toDate = DateOnly.Parse(req.To);

        var fromStart = fromDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var toEnd = toDate.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);

        // Total users in the system
        var totalUsers = await db.Users.AsNoTracking().CountAsync(ct);

        // New users created in the date range
        var newUsers = await db.Users.AsNoTracking()
            .Where(u => u.CreatedAt >= fromStart && u.CreatedAt <= toEnd)
            .CountAsync(ct);

        // User status breakdown
        var activeUsers = await db.Users.AsNoTracking()
            .Where(u => u.Status == UserStatus.Active)
            .CountAsync(ct);

        var suspendedUsers = await db.Users.AsNoTracking()
            .Where(u => u.Status == UserStatus.Suspended)
            .CountAsync(ct);

        var bannedUsers = await db.Users.AsNoTracking()
            .Where(u => u.Status == UserStatus.Banned)
            .CountAsync(ct);

        // Role breakdown (only User and Admin roles)
        var userCount = await db.Users.AsNoTracking()
            .Where(u => u.Role == UserRole.User)
            .CountAsync(ct);

        var adminCount = await db.Users.AsNoTracking()
            .Where(u => u.Role == UserRole.Admin)
            .CountAsync(ct);

        // BookingService is not available - return 0 for booking-related fields
        Response = ApiResponse<Response>.SuccessResult(new Response(
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
        ), "Report summary retrieved");
    }
}

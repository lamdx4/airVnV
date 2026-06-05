using Microsoft.EntityFrameworkCore;
using Airbnb.UserService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Admin.GetUserGrowthReport;

public sealed class Handler(UserDbContext db)
    : Mediator.IQueryHandler<Request, ApiResponse<List<UserGrowthPoint>>>
{
    public async ValueTask<ApiResponse<List<UserGrowthPoint>>> Handle(Request req, CancellationToken ct)
    {
        var fromStart = req.From.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var toEnd = req.To.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);

        // Query new users in range. v1 approximation: UserService UserRole enum
        // is { User, Moderator, Admin } — there is no separate Host/Guest role.
        // We put all new "User" role registrations into the "guests" bucket and
        // leave "hosts" as 0. See BR-004 / UC-E2 spec for context.
        var newUsers = await db.Users.AsNoTracking()
            .Where(u => u.CreatedAt >= fromStart && u.CreatedAt <= toEnd)
            .Where(u => u.Role == Domain.UserRole.User)
            .Select(u => new { u.CreatedAt })
            .ToListAsync(ct);

        var grouped = newUsers
            .GroupBy(u => FormatPeriod(u.CreatedAt, req.GroupBy))
            .OrderBy(g => g.Key)
            .Select(g => new UserGrowthPoint(
                Date: g.Key,
                Guests: g.Count(),
                Hosts: 0
            ))
            .ToList();

        return ApiResponse<List<UserGrowthPoint>>.SuccessResult(grouped);
    }

    private static string FormatPeriod(DateTimeOffset createdAt, string groupBy) =>
        groupBy?.ToLowerInvariant() switch
        {
            "week" => $"W{System.Globalization.ISOWeek.GetWeekOfYear(createdAt.UtcDateTime)}-{createdAt.UtcDateTime.Year}",
            "month" => createdAt.UtcDateTime.ToString("yyyy-MM"),
            _ => createdAt.UtcDateTime.ToString("yyyy-MM-dd")
        };
}

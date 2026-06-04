using Microsoft.EntityFrameworkCore;
using Airbnb.UserService.Infrastructure;
using Airbnb.UserService.Infrastructure.HttpClients;
using Airbnb.ServiceDefaults.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Airbnb.UserService.Features.Admin.GetRecentActivity;

public sealed class Handler(
    UserDbContext db,
    PropertyServiceClient propertyClient,
    ILogger<Handler> logger)
    : Mediator.IQueryHandler<Request, ApiResponse<List<ActivityItem>>>
{
    public async ValueTask<ApiResponse<List<ActivityItem>>> Handle(Request req, CancellationToken ct)
    {
        var limit = Math.Clamp(req.Limit, 1, 50);

        var recentUsers = await db.Users.AsNoTracking()
            .Include(u => u.Profile)
            .OrderByDescending(u => u.CreatedAt)
            .Take(limit)
            .Select(u => new ActivityItem(
                u.Id.ToString(),
                "user",
                $"New user registered: {u.Profile.FullName}",
                u.CreatedAt.ToString("o")
            ))
            .ToListAsync(ct);

        List<ActivityItem> propertyActivities = [];
        try
        {
            var propertyData = await propertyClient.GetRecentActivityAsync(limit, ct);
            if (propertyData is not null)
            {
                propertyActivities = propertyData.Select(p => new ActivityItem(
                    p.Id.ToString(),
                    p.Type,
                    p.Description,
                    p.Timestamp.ToString("o")
                )).ToList();
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to retrieve recent activity from PropertyService");
        }

        var result = recentUsers
            .Concat(propertyActivities)
            .OrderByDescending(a => a.Timestamp)
            .Take(limit)
            .ToList();

        return ApiResponse<List<ActivityItem>>.SuccessResult(result);
    }
}

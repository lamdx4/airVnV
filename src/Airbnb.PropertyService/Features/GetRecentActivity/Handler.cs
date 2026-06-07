using Microsoft.EntityFrameworkCore;
using Airbnb.PropertyService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.GetRecentActivity;

public sealed class Handler(AppDbContext db)
    : Mediator.IQueryHandler<Request, ApiResponse<List<ActivityItem>>>
{
    public async ValueTask<ApiResponse<List<ActivityItem>>> Handle(Request req, CancellationToken ct)
    {
        var limit = Math.Clamp(req.Limit, 1, 50);

        var recentProperties = await db.Properties.AsNoTracking()
            .OrderByDescending(p => p.CreatedAt)
            .Take(limit)
            .Select(p => new ActivityItem(
                p.Id,
                "property",
                $"New property listed: {p.Title}",
                p.CreatedAt
            ))
            .ToListAsync(ct);

        var recentReviews = await db.Reviews.AsNoTracking()
            .OrderByDescending(r => r.CreatedAt)
            .Take(limit)
            .Select(r => new ActivityItem(
                r.Id,
                "review",
                $"New review submitted (Rating: {r.Rating}/5)",
                r.CreatedAt
            ))
            .ToListAsync(ct);

        var result = recentProperties
            .Concat(recentReviews)
            .OrderByDescending(a => a.Timestamp)
            .Take(limit)
            .ToList();

        return ApiResponse<List<ActivityItem>>.SuccessResult(result);
    }
}

using Microsoft.EntityFrameworkCore;
using Airbnb.PropertyService.Infrastructure;
using Airbnb.PropertyService.Domain.Enums;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.GetAdminStats;

public sealed class Handler(AppDbContext db)
    : Mediator.IQueryHandler<Request, ApiResponse<PropertyStatsResponse>>
{
    public async ValueTask<ApiResponse<PropertyStatsResponse>> Handle(Request req, CancellationToken ct)
    {
        var totalProperties = await db.Properties.AsNoTracking().CountAsync(ct);
        var pendingReview = await db.Properties.AsNoTracking().CountAsync(p => p.Status == PropertyStatus.PendingReview, ct);
        var published = await db.Properties.AsNoTracking().CountAsync(p => p.Status == PropertyStatus.Published, ct);
        var suspended = await db.Properties.AsNoTracking().CountAsync(p => p.Status == PropertyStatus.Suspended, ct);
        var totalReviews = await db.Reviews.AsNoTracking().CountAsync(ct);

        return ApiResponse<PropertyStatsResponse>.SuccessResult(
            new PropertyStatsResponse(totalProperties, pendingReview, published, suspended, totalReviews)
        );
    }
}

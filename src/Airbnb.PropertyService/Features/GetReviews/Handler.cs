using Mediator;
using Airbnb.PropertyService.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.PropertyService.Features.GetReviews;

public sealed class Handler(AppDbContext db) : IQueryHandler<GetReviewsRequest, GetReviewsResponse>
{
    public async ValueTask<GetReviewsResponse> Handle(GetReviewsRequest req, CancellationToken ct)
    {
        var page = Math.Max(req.Page, 1);
        var pageSize = Math.Clamp(req.PageSize, 1, 100);
        var skip = (page - 1) * pageSize;

        // Query reviews directly via the navigation property (assuming DbContext can query the shadow entity/owned collection if properly configured)
        // Wait, Review is an owned entity/child entity mapped to the database. We can query it via db.Set<Review>() if it's configured as an Entity,
        // or through db.Properties.SelectMany(p => p.Reviews)
        
        var query = db.Properties
            .AsNoTracking()
            .Where(p => p.Id == req.PropertyId)
            .SelectMany(p => p.Reviews)
            .OrderByDescending(r => r.CreatedAt);

        var totalCount = await query.CountAsync(ct);
        
        var reviews = await query
            .Skip(skip)
            .Take(pageSize)
            .Select(r => new ReviewDto(r.Id, r.GuestId, r.Rating, r.Comment, r.CreatedAt))
            .ToListAsync(ct);

        return new GetReviewsResponse(totalCount, page, pageSize, reviews);
    }
}

using Mediator;
using Airbnb.PropertyService.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.UpdateReview;

public sealed class Handler(AppDbContext db) : ICommandHandler<UpdateReviewRequest, UpdateReviewResponse>
{
    public async ValueTask<UpdateReviewResponse> Handle(UpdateReviewRequest req, CancellationToken ct)
    {
        // Filtered Include: Only load the specific review to save memory
        var property = await db.Properties
            .Include(p => p.Reviews.Where(r => r.Id == req.ReviewId))
            .FirstOrDefaultAsync(p => p.Id == req.PropertyId, ct)
            ?? throw new BusinessException("Property not found.", "PROPERTY_NOT_FOUND");

        property.UpdateReview(req.ReviewId, req.GuestId, req.Rating, req.Comment);

        await db.SaveChangesAsync(ct);

        return new UpdateReviewResponse(req.ReviewId, property.AverageRating, property.ReviewCount);
    }
}

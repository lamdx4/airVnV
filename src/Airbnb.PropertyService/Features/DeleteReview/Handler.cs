using Mediator;
using Airbnb.PropertyService.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.DeleteReview;

public sealed class Handler(AppDbContext db) : ICommandHandler<DeleteReviewRequest, DeleteReviewResponse>
{
    public async ValueTask<DeleteReviewResponse> Handle(DeleteReviewRequest req, CancellationToken ct)
    {
        // Filtered Include: Only load the specific review to save memory
        var property = await db.Properties
            .Include(p => p.Reviews.Where(r => r.Id == req.ReviewId))
            .FirstOrDefaultAsync(p => p.Id == req.PropertyId, ct)
            ?? throw new BusinessException("Property not found.", "PROPERTY_NOT_FOUND");

        property.DeleteReview(req.ReviewId, req.GuestId);

        await db.SaveChangesAsync(ct);

        return new DeleteReviewResponse(req.ReviewId, property.AverageRating, property.ReviewCount);
    }
}

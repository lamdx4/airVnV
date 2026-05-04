using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.PropertyService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;
using Airbnb.PropertyService.Domain.Enums;

namespace Airbnb.PropertyService.Features.UpdateStatus;

public sealed class Handler(AppDbContext db)
    : Mediator.ICommandHandler<Request, ApiResponse<bool>>
{
    public async ValueTask<ApiResponse<bool>> Handle(Request req, CancellationToken ct)
    {
        var property = await db.Properties
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == req.PropertyId && p.HostId == req.RequesterId, ct);

        if (property == null)
            throw new NotFoundException("Property not found or unauthorized.");

        var newStatus = (PropertyStatus)req.Status;

        switch (newStatus)
        {
            case PropertyStatus.Published:
                property.Publish();
                break;
            case PropertyStatus.Archived:
                property.Archive();
                break;
            case PropertyStatus.Draft:
                if (property.Status != PropertyStatus.PendingReview)
                        throw new BusinessException("Cannot revert to Draft from current status.", "PROPERTY_INVALID_STATUS_REVERSION");
                property.UpdateStatus(PropertyStatus.Draft);
                break;
            default:
                property.UpdateStatus(newStatus);
                break;
        }

        await db.SaveChangesAsync(ct);
        return ApiResponse<bool>.SuccessResult(true);
    }
}

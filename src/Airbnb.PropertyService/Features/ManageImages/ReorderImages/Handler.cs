using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.PropertyService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.ManageImages.ReorderImages;

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

        property.ReorderImages(req.Orders);
        
        await db.SaveChangesAsync(ct);
        return ApiResponse<bool>.SuccessResult(true);
    }
}

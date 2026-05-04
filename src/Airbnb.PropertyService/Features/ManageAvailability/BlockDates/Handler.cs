using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.PropertyService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.ManageAvailability.BlockDates;

public sealed class Handler(AppDbContext db)
    : Mediator.ICommandHandler<Request, ApiResponse<bool>>
{
    public async ValueTask<ApiResponse<bool>> Handle(Request req, CancellationToken ct)
    {
        var property = await db.Properties
            .Include(p => p.Availabilities)
            .FirstOrDefaultAsync(p => p.Id == req.PropertyId && p.HostId == req.RequesterId, ct);

        if (property == null)
            throw new NotFoundException("Property not found or unauthorized.");

        property.BlockDates(req.StartDate, req.EndDate, req.Note);
        
        await db.SaveChangesAsync(ct);
        return ApiResponse<bool>.SuccessResult(true);
    }
}

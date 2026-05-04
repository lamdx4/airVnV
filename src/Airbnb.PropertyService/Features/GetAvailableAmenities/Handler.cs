using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.PropertyService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.GetAvailableAmenities;

public sealed class Handler(AppDbContext db)
    : IQueryHandler<Request, ApiResponse<List<AmenityResponse>>>
{
    public async ValueTask<ApiResponse<List<AmenityResponse>>> Handle(Request req, CancellationToken ct)
    {
        var amenities = await db.Amenities
            .AsNoTracking()
            .OrderBy(a => a.Category)
            .ThenBy(a => a.Name)
            .Select(a => new AmenityResponse(a.Id, a.Name, a.Category, a.IconCode))
            .ToListAsync(ct);

        return ApiResponse<List<AmenityResponse>>.SuccessResult(amenities);
    }
}

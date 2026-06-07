using Microsoft.EntityFrameworkCore;
using Airbnb.PropertyService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.GetPropertiesByIds;

public sealed class Handler(AppDbContext db)
    : Mediator.IQueryHandler<Request, ApiResponse<List<PropertyBasicInfo>>>
{
    public async ValueTask<ApiResponse<List<PropertyBasicInfo>>> Handle(Request req, CancellationToken ct)
    {
        if (req.Ids is null || req.Ids.Length == 0)
        {
            return ApiResponse<List<PropertyBasicInfo>>.SuccessResult(new List<PropertyBasicInfo>());
        }

        var distinct = req.Ids.Distinct().ToArray();

        var result = await db.Properties.AsNoTracking()
            .Include(p => p.Images)
            .Where(p => distinct.Contains(p.Id))
            .Select(p => new PropertyBasicInfo(
                p.Id, 
                p.Title,
                p.Pricing.BasePrice,
                "VND",
                p.AverageRating,
                p.DisplayAddress,
                p.Latitude,
                p.Longitude,
                p.Images.OrderBy(i => i.DisplayOrder).Select(i => i.Url.ToString()).ToList()
            ))
            .ToListAsync(ct);

        return ApiResponse<List<PropertyBasicInfo>>.SuccessResult(result);
    }
}

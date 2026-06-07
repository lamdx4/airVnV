using Airbnb.PropertyService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.PropertyService.Features.GetAdminProperties;

namespace Airbnb.PropertyService.Features.GetPublicProperties;

public class Handler(AppDbContext dbContext) : IQueryHandler<Request, PagedResponse<Response>>
{
    public async ValueTask<PagedResponse<Response>> Handle(Request request, CancellationToken ct)
    {
        var query = dbContext.Properties
            .AsNoTracking()
            .Where(p => p.Status == Domain.Enums.PropertyStatus.Published);

        if (request.PropertyType.HasValue)
        {
            query = query.Where(p => (int)p.Type == request.PropertyType.Value);
        }

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new Response(
                p.Id,
                p.Title,
                p.DisplayAddress,
                p.Pricing.BasePrice,
                p.Pricing.CurrencyCode,
                p.AverageRating,
                p.Images.OrderBy(i => i.DisplayOrder).Select(i => i.Url.ToString()).ToList()
            ))
            .ToListAsync(ct);

        return new PagedResponse<Response>(items, totalCount, request.Page, request.PageSize);
    }
}

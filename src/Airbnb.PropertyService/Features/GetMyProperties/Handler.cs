using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.PropertyService.Infrastructure;
using Airbnb.PropertyService.Domain.Enums;

namespace Airbnb.PropertyService.Features.GetMyProperties;

public sealed class Handler(AppDbContext db)
    : IQueryHandler<InternalRequest, PagedResponse<PropertyResponse>>
{
    public async ValueTask<PagedResponse<PropertyResponse>> Handle(InternalRequest req, CancellationToken ct)
    {
        var query = db.Properties
            .AsNoTracking()
            .Where(p => p.HostId == req.RequesterId);

        // Apply filters
        if (!string.IsNullOrWhiteSpace(req.SearchTerm))
        {
            var searchTerm = $"%{req.SearchTerm}%";
            query = query.Where(p => EF.Functions.ILike(p.Title, searchTerm));
        }

        if (req.Status.HasValue)
        {
            query = query.Where(p => (int)p.Status == req.Status.Value);
        }

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(p => p.UpdatedAt ?? p.CreatedAt)
            .Skip((req.PageNumber - 1) * req.PageSize)
            .Take(req.PageSize)
            .Select(p => new PropertyResponse(
                p.Id,
                p.Title,
                p.DisplayAddress,
                (int)p.Status,
                p.Pricing.BasePrice,
                p.Images
                    .Where(i => i.Type == ImageType.Cover)
                    .Select(i => i.Url.ToString())
                    .FirstOrDefault(),
                p.Capacity.GuestCount,
                p.Capacity.BedroomCount,
                p.CreatedAt,
                p.UpdatedAt
            ))
            .ToListAsync(ct);

        return new PagedResponse<PropertyResponse>(items, totalCount, req.PageNumber, req.PageSize);
    }
}

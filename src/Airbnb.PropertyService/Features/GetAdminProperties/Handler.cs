using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.PropertyService.Infrastructure;
using Airbnb.PropertyService.Domain.Enums;

namespace Airbnb.PropertyService.Features.GetAdminProperties;

public sealed class Handler(AppDbContext db) : IQueryHandler<Request, PagedResponse<AdminPropertyResponse>>
{
    public async ValueTask<PagedResponse<AdminPropertyResponse>> Handle(Request req, CancellationToken ct)
    {
        var query = db.Properties.AsNoTracking();

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(req.SearchTerm))
        {
            var searchTerm = $"%{req.SearchTerm}%";
            query = query.Where(p => EF.Functions.ILike(p.Title, searchTerm));
        }

        // Apply status filter
        if (req.Status.HasValue)
        {
            query = query.Where(p => (int)p.Status == req.Status.Value);
        }

        var totalCount = await query.CountAsync(ct);

        // Apply sorting
        query = (req.SortBy?.ToLowerInvariant(), req.SortOrder?.ToLowerInvariant()) switch
        {
            ("title", "asc") => query.OrderBy(p => p.Title),
            ("title", "desc") => query.OrderByDescending(p => p.Title),
            ("price", "asc") => query.OrderBy(p => p.Pricing.BasePrice),
            ("price", "desc") => query.OrderByDescending(p => p.Pricing.BasePrice),
            ("createdat", "asc") => query.OrderBy(p => p.CreatedAt),
            _ => query.OrderByDescending(p => p.UpdatedAt ?? p.CreatedAt),
        };

        var items = await query
            .Skip((req.PageNumber - 1) * req.PageSize)
            .Take(req.PageSize)
            .Select(p => new AdminPropertyResponse(
                p.Id,
                p.HostId,
                p.Title,
                p.DisplayAddress,
                p.Type,
                p.Status,
                p.Pricing.BasePrice,
                p.Images
                    .Where(i => i.Type == ImageType.Cover)
                    .Select(i => i.Url.ToString())
                    .FirstOrDefault(),
                p.Capacity.GuestCount,
                p.Capacity.BedroomCount,
                p.Capacity.BathroomCount,
                p.AverageRating,
                p.ReviewCount,
                p.SuspensionReason,
                p.RejectionReason,
                p.CreatedAt,
                p.UpdatedAt
            ))
            .ToListAsync(ct);

        return new PagedResponse<AdminPropertyResponse>(items, totalCount, req.PageNumber, req.PageSize);
    }
}

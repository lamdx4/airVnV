using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.PropertyService.Infrastructure;
using Airbnb.PropertyService.Domain.Enums;

namespace Airbnb.PropertyService.Features.AdminPropertyModeration.GetPendingProperties;

public sealed class Handler(AppDbContext db) : IQueryHandler<Request, Response>
{
    public async ValueTask<Response> Handle(Request req, CancellationToken ct)
    {
        var query = db.Properties
            .AsNoTracking()
            .Where(p => p.Status == PropertyStatus.PendingReview);

        // Search by title
        if (!string.IsNullOrWhiteSpace(req.Search))
        {
            query = query.Where(p => p.Title.ToLower().Contains(req.Search.ToLower()));
        }

        // Get total count
        var totalCount = await query.CountAsync(ct);

        // Sorting
        query = req.SortBy?.ToLower() switch
        {
            "title" => req.SortOrder == "asc"
                ? query.OrderBy(p => p.Title)
                : query.OrderByDescending(p => p.Title),
            "hostname" => req.SortOrder == "asc"
                ? query.OrderBy(p => p.HostId)
                : query.OrderByDescending(p => p.HostId),
            _ => req.SortOrder == "asc"
                ? query.OrderBy(p => p.CreatedAt)
                : query.OrderByDescending(p => p.CreatedAt)
        };

        // Pagination
        var skip = (req.Page - 1) * req.PageSize;
        var properties = await query
            .Skip(skip)
            .Take(req.PageSize)
            .Select(p => new PropertySummaryDto(
                p.Id,
                p.Title,
                p.Images.FirstOrDefault(i => i.Type == ImageType.Cover).Url.ToString() ?? "",
                p.HostId.ToString(),
                p.CreatedAt,
                p.Status.ToString()
            ))
            .ToListAsync(ct);

        return new Response(properties, totalCount, req.Page, req.PageSize);
    }
}
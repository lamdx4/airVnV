using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.PropertyService.Infrastructure;
using Airbnb.PropertyService.Domain.Enums;

namespace Airbnb.PropertyService.Features.Admin.Reports.GetStatusFunnel;

public sealed class GetStatusFunnelHandler(AppDbContext db)
    : IQueryHandler<Request, Response>
{
    public async ValueTask<Response> Handle(Request req, CancellationToken ct)
    {
        var counts = await db.Properties.AsNoTracking()
            .GroupBy(p => p.Status)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToListAsync(ct);

        int Get(PropertyStatus s) => counts.FirstOrDefault(c => c.Key == s)?.Count ?? 0;

        return new Response(
            Draft: Get(PropertyStatus.Draft),
            PendingReview: Get(PropertyStatus.PendingReview),
            Published: Get(PropertyStatus.Published),
            Suspended: Get(PropertyStatus.Suspended),
            Rejected: Get(PropertyStatus.Rejected),
            Archived: Get(PropertyStatus.Archived)
        );
    }
}

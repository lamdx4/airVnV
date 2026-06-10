using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.PropertyService.Infrastructure;
using Airbnb.PropertyService.Domain.Enums;

namespace Airbnb.PropertyService.Features.Admin.Reports.GetTypeDistribution;

public sealed class GetTypeDistributionHandler(AppDbContext db)
    : IQueryHandler<Request, List<TypeCount>>
{
    public async ValueTask<List<TypeCount>> Handle(Request req, CancellationToken ct)
    {
        var counts = await db.Properties.AsNoTracking()
            .GroupBy(p => p.Type)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToListAsync(ct);

        return Enum.GetValues<PropertyType>()
            .Select(t => new TypeCount(t.ToString(), counts.FirstOrDefault(c => c.Key == t)?.Count ?? 0))
            .ToList();
    }
}

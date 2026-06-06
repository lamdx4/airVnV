using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Airbnb.PropertyService.Infrastructure;
using Airbnb.PropertyService.Domain.Enums;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.Admin.Reports.GetTypeDistribution;

public record TypeCount(string Type, int Count);

public class Endpoint(AppDbContext db) : EndpointWithoutRequest<ApiResponse<List<TypeCount>>>
{
    public override void Configure()
    {
        Get("/api/properties/admin/reports/type-distribution");
        AllowAnonymous();
        Summary(s => s.Summary = "Admin: property counts grouped by type");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var counts = await db.Properties.AsNoTracking()
            .GroupBy(p => p.Type)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToListAsync(ct);

        var result = Enum.GetValues<PropertyType>()
            .Select(t => new TypeCount(t.ToString(), counts.FirstOrDefault(c => c.Key == t)?.Count ?? 0))
            .ToList();

        await Send.ResponseAsync(ApiResponse<List<TypeCount>>.SuccessResult(result), cancellation: ct);
    }
}

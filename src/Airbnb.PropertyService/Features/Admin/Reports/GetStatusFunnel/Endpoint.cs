using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Airbnb.PropertyService.Infrastructure;
using Airbnb.PropertyService.Domain.Enums;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.Admin.Reports.GetStatusFunnel;

public record Response(
    int Draft,
    int PendingReview,
    int Published,
    int Suspended,
    int Rejected,
    int Archived
);

public class Endpoint(AppDbContext db) : EndpointWithoutRequest<ApiResponse<Response>>
{
    public override void Configure()
    {
        Get("/api/properties/admin/reports/status-funnel");
        AllowAnonymous();
        Summary(s => s.Summary = "Admin: property counts grouped by status");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var counts = await db.Properties.AsNoTracking()
            .GroupBy(p => p.Status)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToListAsync(ct);

        int Get(PropertyStatus s) => counts.FirstOrDefault(c => c.Key == s)?.Count ?? 0;

        var response = new Response(
            Draft: Get(PropertyStatus.Draft),
            PendingReview: Get(PropertyStatus.PendingReview),
            Published: Get(PropertyStatus.Published),
            Suspended: Get(PropertyStatus.Suspended),
            Rejected: Get(PropertyStatus.Rejected),
            Archived: Get(PropertyStatus.Archived)
        );

        await Send.ResponseAsync(ApiResponse<Response>.SuccessResult(response), cancellation: ct);
    }
}

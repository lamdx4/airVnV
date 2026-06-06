using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Airbnb.PropertyService.Infrastructure;
using Airbnb.PropertyService.Domain.Enums;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.Admin.Reports.GetPendingBacklog;

public record Response(
    int PendingCount,
    double AverageWaitDays,
    double MaxWaitDays,
    int OverdueCount,
    int OverdueThresholdDays
);

public class Endpoint(AppDbContext db) : EndpointWithoutRequest<ApiResponse<Response>>
{
    private const int OverdueDays = 3;

    public override void Configure()
    {
        Get("/api/properties/admin/reports/pending-backlog");
        AllowAnonymous();
        Summary(s => s.Summary = "Admin: pending-review backlog and SLA");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var now = DateTimeOffset.UtcNow;
        var threshold = now.AddDays(-OverdueDays);

        var pending = await db.Properties.AsNoTracking()
            .Where(p => p.Status == PropertyStatus.PendingReview)
            .Select(p => p.CreatedAt)
            .ToListAsync(ct);

        var count = pending.Count;
        double avg = 0, max = 0;
        var overdue = 0;
        if (count > 0)
        {
            var spans = pending.Select(c => (now - c).TotalDays).ToList();
            avg = spans.Average();
            max = spans.Max();
            overdue = pending.Count(c => c <= threshold);
        }

        var response = new Response(
            PendingCount: count,
            AverageWaitDays: Math.Round(avg, 2),
            MaxWaitDays: Math.Round(max, 2),
            OverdueCount: overdue,
            OverdueThresholdDays: OverdueDays
        );

        await Send.ResponseAsync(ApiResponse<Response>.SuccessResult(response), cancellation: ct);
    }
}

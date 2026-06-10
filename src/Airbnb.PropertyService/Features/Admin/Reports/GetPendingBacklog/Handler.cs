using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.PropertyService.Infrastructure;
using Airbnb.PropertyService.Domain.Enums;

namespace Airbnb.PropertyService.Features.Admin.Reports.GetPendingBacklog;

public sealed class GetPendingBacklogHandler(AppDbContext db)
    : IQueryHandler<Request, Response>
{
    private const int OverdueDays = 3;

    public async ValueTask<Response> Handle(Request req, CancellationToken ct)
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

        return new Response(
            PendingCount: count,
            AverageWaitDays: Math.Round(avg, 2),
            MaxWaitDays: Math.Round(max, 2),
            OverdueCount: overdue,
            OverdueThresholdDays: OverdueDays
        );
    }
}

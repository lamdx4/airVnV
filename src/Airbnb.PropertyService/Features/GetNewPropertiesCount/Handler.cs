using Microsoft.EntityFrameworkCore;
using Airbnb.PropertyService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.GetNewPropertiesCount;

public sealed class Handler(AppDbContext db)
    : Mediator.IQueryHandler<Request, ApiResponse<NewPropertiesCountResponse>>
{
    public async ValueTask<ApiResponse<NewPropertiesCountResponse>> Handle(Request req, CancellationToken ct)
    {
        var fromStart = req.From.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var toEnd = req.To.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);

        var newProperties = await db.Properties.AsNoTracking()
            .Where(p => p.CreatedAt >= fromStart && p.CreatedAt <= toEnd)
            .CountAsync(ct);

        return ApiResponse<NewPropertiesCountResponse>.SuccessResult(new NewPropertiesCountResponse(newProperties));
    }
}

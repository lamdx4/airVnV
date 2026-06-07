using Microsoft.EntityFrameworkCore;
using Airbnb.PropertyService.Infrastructure;
using Airbnb.PropertyService.Domain.Enums;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.GetPublishedCount;

public sealed class Handler(AppDbContext db)
    : Mediator.IQueryHandler<Request, ApiResponse<PublishedCountResponse>>
{
    public async ValueTask<ApiResponse<PublishedCountResponse>> Handle(Request req, CancellationToken ct)
    {
        var published = await db.Properties.AsNoTracking()
            .CountAsync(p => p.Status == PropertyStatus.Published, ct);

        return ApiResponse<PublishedCountResponse>.SuccessResult(new PublishedCountResponse(published));
    }
}

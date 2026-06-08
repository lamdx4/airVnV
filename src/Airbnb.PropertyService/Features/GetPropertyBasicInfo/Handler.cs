using Airbnb.PropertyService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;
using Mediator;
using Microsoft.EntityFrameworkCore;

using Airbnb.PropertyService.Domain.ValueObjects;

namespace Airbnb.PropertyService.Features.GetPropertyBasicInfo;

public sealed class Handler(AppDbContext db) : IQueryHandler<Request, Response>
{
    public async ValueTask<Response> Handle(Request req, CancellationToken ct)
    {
        var p = await db.Properties
            .AsNoTracking()
            .Where(x => x.Id == req.PropertyId)
            .Select(x => new { x.Id, x.Title, x.Description, x.HostId, x.Pricing, x.Capacity, x.HouseRules, x.CountryCode, x.Type, x.BookingMode })
            .FirstOrDefaultAsync(ct);

        if (p == null)
        {
            throw new NotFoundException("Property not found.");
        }

        return new Response(p.Id, p.Title, p.Description, p.HostId, p.Pricing, p.Capacity, p.HouseRules, p.CountryCode, p.Type, p.BookingMode.ToString());
    }
}

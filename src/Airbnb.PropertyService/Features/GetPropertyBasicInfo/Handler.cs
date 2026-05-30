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
        var property = await db.Properties
            .AsNoTracking()
            .Where(p => p.Id == req.PropertyId)
            .Select(p => new Response(p.Id, p.Title, p.Description, p.HostId, p.Pricing, p.Capacity, p.HouseRules, p.CountryCode, p.Type, p.BookingMode))
            .FirstOrDefaultAsync(ct);

        if (property == null)
        {
            throw new NotFoundException("Property not found.");
        }

        return property;
    }
}

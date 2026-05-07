using Airbnb.PropertyService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;
using Airbnb.SharedKernel.Events;
using MassTransit;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.PropertyService.Features.ToggleCountrySupport;

public sealed class Handler(AppDbContext db, IPublishEndpoint publishEndpoint) : ICommandHandler<Request, Response>
{
    public async ValueTask<Response> Handle(Request req, CancellationToken ct)
    {
        var countryCode = req.CountryCode.ToUpperInvariant();
        
        var country = await db.Countries.FirstOrDefaultAsync(c => c.Code == countryCode, ct);

        if (country == null)
            throw new NotFoundException($"Country {countryCode} not found.");

        country.ToggleSupport();

        await db.SaveChangesAsync(ct);

        // Publish event for cache invalidation
        await publishEndpoint.Publish(new CountryToggledEvent(country.Code, country.IsSupported), ct);

        return new Response(country.Code, country.IsSupported);
    }
}

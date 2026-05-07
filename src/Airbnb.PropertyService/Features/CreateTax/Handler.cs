using Airbnb.PropertyService.Domain.Entities;
using Airbnb.PropertyService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;
using Airbnb.SharedKernel.Events;
using MassTransit;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.PropertyService.Features.CreateTax;

public sealed class Handler(AppDbContext db, IPublishEndpoint publishEndpoint) : ICommandHandler<Request, Response>
{
    public async ValueTask<Response> Handle(Request req, CancellationToken ct)
    {
        var countryCode = req.CountryCode.ToUpperInvariant();
        
        var country = await db.Countries.FirstOrDefaultAsync(c => c.Code == countryCode, ct);
        if (country == null)
            throw new BusinessException($"Country {countryCode} does not exist. Please create country first.", "COUNTRY_NOT_FOUND");

        var tax = Tax.Create(countryCode, req.Type, req.Rate, req.IsActive);
        db.Taxes.Add(tax);

        await db.SaveChangesAsync(ct);

        await publishEndpoint.Publish(new TaxUpdatedEvent(countryCode), ct);

        return new Response(tax.Id, tax.CountryCode, tax.Type, tax.Rate, tax.IsActive);
    }
}

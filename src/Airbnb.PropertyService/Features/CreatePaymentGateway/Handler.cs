using Airbnb.PropertyService.Domain.Entities;
using Airbnb.PropertyService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;
using Airbnb.SharedKernel.Events;
using MassTransit;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.PropertyService.Features.CreatePaymentGateway;

public sealed class Handler(AppDbContext db, IPublishEndpoint publishEndpoint) : ICommandHandler<Request, Response>
{
    public async ValueTask<Response> Handle(Request req, CancellationToken ct)
    {
        var countryCode = req.CountryCode.ToUpperInvariant();
        
        var country = await db.Countries.FirstOrDefaultAsync(c => c.Code == countryCode, ct);
        if (country == null)
            throw new BusinessException($"Country {countryCode} does not exist.", "COUNTRY_NOT_FOUND");

        var gateway = PaymentGateway.Create(countryCode, req.Provider, req.SupportedCurrencies, req.IsActive);
        db.PaymentGateways.Add(gateway);

        await db.SaveChangesAsync(ct);

        await publishEndpoint.Publish(new PaymentGatewayUpdatedEvent(countryCode), ct);

        return new Response(gateway.Id, gateway.CountryCode, gateway.Provider, gateway.SupportedCurrencies, gateway.IsActive);
    }
}

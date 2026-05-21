using Airbnb.PropertyService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.PropertyService.Features.GetCountryMasterData;

public sealed class Handler(AppDbContext db) : IQueryHandler<Request, CountryMasterDataDto>
{
    public async ValueTask<CountryMasterDataDto> Handle(Request req, CancellationToken ct)
    {
        var countryCode = req.CountryCode.ToUpperInvariant();
        
        var country = await db.Countries
            .AsNoTracking()
            .Include(c => c.Taxes)
            .Include(c => c.PaymentGateways)
            .FirstOrDefaultAsync(c => c.Code == countryCode, ct);

        if (country == null)
            throw new NotFoundException($"Country master data for {countryCode} not found.");

        return new CountryMasterDataDto(
            country.Code,
            country.Name,
            country.NativeCurrency,
            country.IsSupported,
            country.DefaultLatitude,
            country.DefaultLongitude,
            country.Taxes.Where(t => t.IsActive).Select(t => new TaxDto(t.Type, t.Rate)).ToList(),
            country.PaymentGateways.Where(g => g.IsActive).Select(g => new PaymentGatewayDto(g.Provider, g.SupportedCurrencies)).ToList(),
            country.AddressFormConfig
        );
    }
}

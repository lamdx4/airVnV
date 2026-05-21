using Airbnb.PropertyService.Infrastructure;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.PropertyService.Features.GetSupportedCountries;

public sealed class Handler(AppDbContext db) : IQueryHandler<Request, List<SupportedCountryDto>>
{
    public async ValueTask<List<SupportedCountryDto>> Handle(Request req, CancellationToken ct)
    {
        var countries = await db.Countries
            .AsNoTracking()
            .Where(c => c.IsSupported)
            .OrderBy(c => c.Name)
            .Select(c => new SupportedCountryDto(
                c.Code,
                c.Name,
                c.NativeCurrency,
                c.DefaultLatitude,
                c.DefaultLongitude
            ))
            .ToListAsync(ct);

        return countries;
    }
}

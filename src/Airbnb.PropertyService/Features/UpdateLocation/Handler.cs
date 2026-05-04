using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.PropertyService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;
using Airbnb.PropertyService.Domain.ValueObjects;

namespace Airbnb.PropertyService.Features.UpdateLocation;

public sealed class Handler(AppDbContext db)
    : Mediator.ICommandHandler<Request, ApiResponse<bool>>
{
    public async ValueTask<ApiResponse<bool>> Handle(Request req, CancellationToken ct)
    {
        var property = await db.Properties
            .FirstOrDefaultAsync(p => p.Id == req.PropertyId && p.HostId == req.RequesterId, ct);

        if (property == null)
            throw new NotFoundException("Property not found or unauthorized.");

        var addressRaw = new AddressRaw(
            req.StreetAddress,
            req.Unit,
            req.PostalCode,
            req.SubDivisions,
            null
        );

        property.UpdateLocation(
            req.Latitude,
            req.Longitude,
            req.CountryCode,
            req.DisplayAddress,
            addressRaw,
            req.Admin1Code,
            req.Admin2Code
        );

        await db.SaveChangesAsync(ct);
        return ApiResponse<bool>.SuccessResult(true);
    }
}

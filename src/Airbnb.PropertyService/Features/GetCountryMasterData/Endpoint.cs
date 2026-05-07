using Airbnb.ServiceDefaults.Infrastructure;
using FastEndpoints;
using Mediator;

namespace Airbnb.PropertyService.Features.GetCountryMasterData;

public class Endpoint(IMediator mediator) : Endpoint<Request, ApiResponse<CountryMasterDataDto>>
{
    public override void Configure()
    {
        Get("/api/internal/master-data/countries/{CountryCode}");
        AllowAnonymous(); // Internal API calls usually bypass user auth, but might need service-to-service auth in production
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await SendAsync(ApiResponse<CountryMasterDataDto>.SuccessResult(result), cancellation: ct);
    }
}

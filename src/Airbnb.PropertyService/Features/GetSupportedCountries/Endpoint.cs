using Airbnb.ServiceDefaults.Infrastructure;
using FastEndpoints;
using Mediator;

namespace Airbnb.PropertyService.Features.GetSupportedCountries;

public class Endpoint(IMediator mediator) : EndpointWithoutRequest<ApiResponse<List<SupportedCountryDto>>>
{
    public override void Configure()
    {
        Get("/api/properties/countries");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await mediator.Send(new Request(), ct);
        await Send.ResponseAsync(ApiResponse<List<SupportedCountryDto>>.SuccessResult(result), cancellation: ct);
    }
}

using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.Admin.Reports.GetTypeDistribution;

public class Endpoint(IMediator mediator) : EndpointWithoutRequest<ApiResponse<List<TypeCount>>>
{
    public override void Configure()
    {
        Get("/api/properties/admin/reports/type-distribution");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Admin: property counts grouped by type";
            s.Description = "No error codes. Read-only aggregate.";
            s.Responses[200] = "Type distribution retrieved";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await mediator.Send(new Request(), ct);
        Response = ApiResponse<List<TypeCount>>.SuccessResult(result);
    }
}

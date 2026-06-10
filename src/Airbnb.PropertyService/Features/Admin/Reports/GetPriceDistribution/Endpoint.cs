using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.Admin.Reports.GetPriceDistribution;

public class Endpoint(IMediator mediator) : Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Get("/api/properties/admin/reports/price-distribution");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Admin: base price histogram for published listings";
            s.Description = "No error codes. Read-only aggregate (buckets + median/p90/avg).";
            s.Responses[200] = "Price distribution retrieved";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        Response = ApiResponse<Response>.SuccessResult(result);
    }
}

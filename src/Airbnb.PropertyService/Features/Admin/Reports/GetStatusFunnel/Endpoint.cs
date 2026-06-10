using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.Admin.Reports.GetStatusFunnel;

public class Endpoint(IMediator mediator) : Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Get("/api/properties/admin/reports/status-funnel");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Admin: property counts grouped by status";
            s.Description = "No error codes. Read-only aggregate.";
            s.Responses[200] = "Status funnel retrieved";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        Response = ApiResponse<Response>.SuccessResult(result);
    }
}

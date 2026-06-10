using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.Admin.Reports.GetPendingBacklog;

public class Endpoint(IMediator mediator) : EndpointWithoutRequest<ApiResponse<Response>>
{
    public override void Configure()
    {
        Get("/api/properties/admin/reports/pending-backlog");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Admin: pending-review backlog and SLA";
            s.Description = "No error codes. Read-only aggregate over `PendingReview` properties.";
            s.Responses[200] = "Pending backlog retrieved";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await mediator.Send(new Request(), ct);
        Response = ApiResponse<Response>.SuccessResult(result);
    }
}

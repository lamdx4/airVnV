using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.Admin.Reports.GetNewListings;

public class Endpoint(IMediator mediator) : Endpoint<Request, ApiResponse<List<Response>>>
{
    public override void Configure()
    {
        Get("/api/properties/admin/reports/new-listings");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Admin: new listings time series";
            s.Description = "**Possible error codes:**\n- `INVALID_DATE_RANGE` — `From`/`To` is not a valid yyyy-MM-dd date";
            s.Responses[200] = "New listings retrieved";
            s.Responses[400] = "Invalid date range";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        Response = ApiResponse<List<Response>>.SuccessResult(result);
    }
}

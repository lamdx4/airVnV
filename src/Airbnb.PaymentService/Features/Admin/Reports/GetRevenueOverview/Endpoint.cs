using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Features.Admin.Reports.GetRevenueOverview;

public class Endpoint(IMediator mediator) : Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Get("/api/admin/payments/reports/revenue-overview");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Admin: revenue overview (GMV, net revenue) within a date range";
            s.Description = "**Possible error codes:**\n- `INVALID_DATE_RANGE` — `From`/`To` is not a valid yyyy-MM-dd date";
            s.Responses[200] = "Revenue overview retrieved";
            s.Responses[400] = "Invalid date range";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        Response = ApiResponse<Response>.SuccessResult(result);
    }
}

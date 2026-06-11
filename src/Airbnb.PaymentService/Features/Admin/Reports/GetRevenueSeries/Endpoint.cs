using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Features.Admin.Reports.GetRevenueSeries;

public class Endpoint(IMediator mediator) : Endpoint<Request, ApiResponse<List<RevenuePoint>>>
{
    public override void Configure()
    {
        Get("/api/admin/payments/reports/revenue-series");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Admin: revenue time series (GMV + net revenue) by day/week/month";
            s.Description = "**Possible error codes:**\n- `INVALID_DATE_RANGE` — `From`/`To` is not a valid yyyy-MM-dd date";
            s.Responses[200] = "Revenue series retrieved";
            s.Responses[400] = "Invalid date range";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        Response = ApiResponse<List<RevenuePoint>>.SuccessResult(result);
    }
}

using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Admin.GetReportSummary;

public class Endpoint(IMediator mediator) : Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Get("/summary");
        Group<ReportsGroup>();
        Roles("Admin", "Moderator");
        Summary(s =>
        {
            s.Summary = "Admin: report summary across users, bookings, revenue";
            s.Description = "**Possible error codes:**\n- `INVALID_DATE_RANGE` — `From`/`To` is not a valid yyyy-MM-dd date";
            s.Responses[200] = "Report summary retrieved";
            s.Responses[400] = "Invalid date range";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        Response = ApiResponse<Response>.SuccessResult(result, "Report summary retrieved");
    }
}

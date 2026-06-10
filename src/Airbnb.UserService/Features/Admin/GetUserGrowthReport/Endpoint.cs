using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Admin.GetUserGrowthReport;

public class Endpoint(IMediator mediator) : Endpoint<Request, ApiResponse<List<Response>>>
{
    public override void Configure()
    {
        Get("/user-growth");
        Group<ReportsGroup>();
        Roles("Admin", "Moderator");
        Summary(s =>
        {
            s.Summary = "Admin: user growth time series (day/week/month)";
            s.Description = "**Possible error codes:**\n- `INVALID_DATE_RANGE` — `From`/`To` is not a valid yyyy-MM-dd date";
            s.Responses[200] = "User growth retrieved";
            s.Responses[400] = "Invalid date range";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        Response = ApiResponse<List<Response>>.SuccessResult(result, "User growth retrieved");
    }
}

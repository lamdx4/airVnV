using FastEndpoints;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Admin.GetTopPropertiesReport;

public class Endpoint : Endpoint<Request, ApiResponse<List<Response>>>
{
    public override void Configure()
    {
        Get("/top-properties");
        Group<ReportsGroup>();
        Roles("Admin", "Moderator");
        Summary(s =>
        {
            s.Summary = "Admin: top properties placeholder (PropertyService not wired)";
            s.Description = "No error codes. Returns empty list until PropertyService integration lands.";
            s.Responses[200] = "Top properties retrieved";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        Response = ApiResponse<List<Response>>.SuccessResult([], "Top properties retrieved");
        await Task.CompletedTask;
    }
}

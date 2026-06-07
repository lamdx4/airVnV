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
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        Response = ApiResponse<List<Response>>.SuccessResult([], "Top properties retrieved");
    }
}

using FastEndpoints;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Admin.GetUserGrowthReport;

public class Endpoint : Endpoint<Request, ApiResponse<List<Response>>>
{
    public override void Configure()
    {
        Get("/user-growth");
        Group<ReportsGroup>();
        Roles("Admin", "Moderator");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        Response = ApiResponse<List<Response>>.SuccessResult([], "User growth retrieved");
    }
}

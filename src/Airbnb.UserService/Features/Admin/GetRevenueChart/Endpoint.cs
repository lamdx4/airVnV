using FastEndpoints;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Admin.GetRevenueChart;

public class Endpoint : Endpoint<Request, ApiResponse<List<Response>>>
{
    public override void Configure()
    {
        Get("/revenue-breakdown");
        Group<ReportsGroup>();
        Roles("Admin", "Moderator");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        Response = ApiResponse<List<Response>>.SuccessResult([], "Revenue breakdown retrieved");
    }
}

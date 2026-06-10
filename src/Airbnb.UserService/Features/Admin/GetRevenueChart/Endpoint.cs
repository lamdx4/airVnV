using FastEndpoints;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Admin.GetRevenueChart;

// Placeholder endpoint (Rule #7 Simplicity Guard): no DB, no domain logic — static empty list.
public class Endpoint : Endpoint<Request, ApiResponse<List<Response>>>
{
    public override void Configure()
    {
        Get("/revenue-breakdown");
        Group<ReportsGroup>();
        Roles("Admin", "Moderator");
        Summary(s =>
        {
            s.Summary = "Admin: revenue breakdown placeholder (BookingService not wired)";
            s.Description = "No error codes. Returns empty list until BookingService integration lands.";
            s.Responses[200] = "Revenue breakdown retrieved";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        Response = ApiResponse<List<Response>>.SuccessResult([], "Revenue breakdown retrieved");
        await Task.CompletedTask;
    }
}

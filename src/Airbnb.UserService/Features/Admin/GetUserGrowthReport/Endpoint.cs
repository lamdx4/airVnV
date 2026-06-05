using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Admin.GetUserGrowthReport;

public class Endpoint(IMediator mediator)
    : FastEndpoints.Endpoint<Request, ApiResponse<List<UserGrowthPoint>>>
{
    public override void Configure()
    {
        Get("/user-growth");
        Group<ReportsGroup>();
        Roles("Admin", "Moderator");
        Summary(s =>
        {
            s.Summary = "Admin: get user growth (guests vs hosts) per period";
            s.Description = "Returns new user counts grouped by day/week/month. v1 approximation: the UserService UserRole enum has only User/Moderator/Admin, so 'guests' is all new users and 'hosts' is 0. See BR-004 Q2/BR-001 user-management for the data-model limitation.";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await Send.ResponseAsync(result, cancellation: ct);
    }
}

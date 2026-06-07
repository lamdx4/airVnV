using FastEndpoints;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Admin.GetUsers;

public class Endpoint : Endpoint<Request, ApiResponse<PaginatedUserListResponse>>
{
    public override void Configure()
    {
        Get("/");
        Group<AdminGroup>();
        Roles("Admin", "Moderator");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        Response = await req.ExecuteAsync(ct);
    }
}

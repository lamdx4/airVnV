using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Admin.GetUsers;

public class Endpoint(IMediator mediator) : Endpoint<Request, ApiResponse<PaginatedUserListResponse>>
{
    public override void Configure()
    {
        Get("/");
        Group<AdminGroup>();
        Roles("Admin", "Moderator");
        Summary(s =>
        {
            s.Summary = "Admin: list users with filter, search and pagination";
            s.Description = "**Possible error codes:**\n- `VALIDATION_ERROR` — invalid query parameters";
            s.Responses[200] = "Users retrieved successfully";
            s.Responses[400] = "Validation error";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        Response = ApiResponse<PaginatedUserListResponse>.SuccessResult(result, "Users retrieved successfully");
    }
}

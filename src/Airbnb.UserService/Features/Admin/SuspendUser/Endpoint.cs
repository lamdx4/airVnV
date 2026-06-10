using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Admin.SuspendUser;

public class Endpoint(IMediator mediator) : Endpoint<Request, ApiResponse<UserActionResponse>>
{
    public override void Configure()
    {
        Patch("/{Id}/suspend");
        Group<AdminGroup>();
        Roles("Admin", "Moderator");
        Summary(s =>
        {
            s.Summary = "Admin: suspend a user";
            s.Description = "**Possible error codes:**\n- `NOT_FOUND` — user does not exist\n- `INVALID_STATUS_TRANSITION` — only active users can be suspended\n- `REASON_REQUIRED` — suspension reason is empty";
            s.Responses[200] = "User suspended successfully";
            s.Responses[400] = "Business rule violation";
            s.Responses[404] = "User not found";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var id = Route<Guid>("Id");
        var result = await mediator.Send(req with { Id = id }, ct);
        Response = ApiResponse<UserActionResponse>.SuccessResult(result, "User suspended");
    }
}

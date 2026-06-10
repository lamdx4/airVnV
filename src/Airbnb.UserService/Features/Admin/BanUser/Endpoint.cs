using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Admin.BanUser;

public class Endpoint(IMediator mediator) : Endpoint<Request, ApiResponse<BanUserResponse>>
{
    public override void Configure()
    {
        Patch("/{Id}/ban");
        Group<AdminGroup>();
        Roles("Admin", "Moderator");
        Summary(s =>
        {
            s.Summary = "Admin: ban a user";
            s.Description = "**Possible error codes:**\n- `NOT_FOUND` — user does not exist\n- `INVALID_STATUS_TRANSITION` — user current state cannot be banned\n- `USER_ALREADY_BANNED` — user is already banned\n- `REASON_REQUIRED` — ban reason is empty";
            s.Responses[200] = "User banned successfully";
            s.Responses[400] = "Business rule violation";
            s.Responses[404] = "User not found";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var id = Route<Guid>("Id");
        var result = await mediator.Send(req with { Id = id }, ct);
        Response = ApiResponse<BanUserResponse>.SuccessResult(result, "User banned");
    }
}

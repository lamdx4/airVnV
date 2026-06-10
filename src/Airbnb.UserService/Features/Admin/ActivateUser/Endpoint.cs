using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Admin.ActivateUser;

public class Endpoint(IMediator mediator) : EndpointWithoutRequest<ApiResponse<ActivateUserResponse>>
{
    public override void Configure()
    {
        Patch("/{Id}/activate");
        Group<AdminGroup>();
        Roles("Admin", "Moderator");
        Summary(s =>
        {
            s.Summary = "Admin: activate a suspended or banned user";
            s.Description = "**Possible error codes:**\n- `NOT_FOUND` — user does not exist\n- `INVALID_STATUS_TRANSITION` — only suspended or banned users can be activated";
            s.Responses[200] = "User activated successfully";
            s.Responses[400] = "Business rule violation";
            s.Responses[404] = "User not found";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("Id");
        var result = await mediator.Send(new ActivateUserRequest(id), ct);
        Response = ApiResponse<ActivateUserResponse>.SuccessResult(result, "User activated");
    }
}

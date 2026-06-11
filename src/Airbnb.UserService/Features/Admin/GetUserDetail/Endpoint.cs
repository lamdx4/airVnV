using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Admin.GetUserDetail;

public class Endpoint(IMediator mediator) : EndpointWithoutRequest<ApiResponse<UserDetailResponse>>
{
    public override void Configure()
    {
        Get("/{Id}");
        Group<AdminGroup>();
        Roles("Admin", "Moderator");
        Summary(s =>
        {
            s.Summary = "Admin: get user detail by id";
            s.Description = "**Possible error codes:**\n- `NOT_FOUND` — user does not exist";
            s.Responses[200] = "User detail retrieved";
            s.Responses[404] = "User not found";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("Id");
        var result = await mediator.Send(new GetUserDetailRequest(id), ct);
        Response = ApiResponse<UserDetailResponse>.SuccessResult(result, "User detail retrieved");
    }
}

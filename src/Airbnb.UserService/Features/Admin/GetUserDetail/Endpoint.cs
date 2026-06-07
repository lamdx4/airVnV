using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Admin.GetUserDetail;

public class Endpoint : EndpointWithoutRequest<ApiResponse<UserDetailResponse>>
{
    private readonly IMediator _mediator;

    public Endpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/{Id}");
        Group<AdminGroup>();
        Roles("Admin", "Moderator");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("Id");
        var result = await _mediator.Send(new GetUserDetailRequest(id), ct);

        if (result is null)
        {
            await Send.ResponseAsync(ApiResponse<UserDetailResponse>.FailureResult("USER_NOT_FOUND", "User not found"), 404, ct);
            return;
        }

        Response = result;
    }
}

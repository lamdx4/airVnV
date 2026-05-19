using FastEndpoints;
using Mediator;
using System.Security.Claims;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Profile.Get;

public class Endpoint(IMediator mediator) : EndpointWithoutRequest<ApiResponse<Response>>
{
    public override void Configure()
    {
        Get("/api/users/me");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userIdClaim = User.FindFirstValue("UserId");
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            await Send.ResponseAsync(null!, 401, ct);
            return;
        }

        var data = await mediator.Send(new GetProfileRequest(userId), ct);

        if (data == null)
        {
            await Send.ResponseAsync(null!, 404, ct);
            return;
        }

        Response = ApiResponse<Response>.SuccessResult(data);
    }
}

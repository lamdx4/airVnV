using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Profile.GetPublic;

public class Endpoint(IMediator mediator) : EndpointWithoutRequest<ApiResponse<Response>>
{
    public override void Configure()
    {
        Get("/api/users/{id}/public-profile");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var idStr = Route<string>("id");
        if (!Guid.TryParse(idStr, out var userId))
        {
            await Send.ResponseAsync(null!, 400, ct);
            return;
        }

        var data = await mediator.Send(new Request(userId), ct);

        if (data == null)
        {
            await Send.ResponseAsync(null!, 404, ct);
            return;
        }

        Response = ApiResponse<Response>.SuccessResult(data);
    }
}

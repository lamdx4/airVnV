using FastEndpoints;

using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Login.Login;

public class Endpoint(Mediator.IMediator mediator) : Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Post("/api/users/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        Response = await mediator.Send(req, ct);
    }
}

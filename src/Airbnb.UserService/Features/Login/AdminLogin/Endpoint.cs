using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Login.AdminLogin;

public class Endpoint(IMediator mediator) : Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Post("/api/users/admin/login");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Admin login (Admin or Moderator role required)";
            s.Description = "**Possible error codes:**\n- `AUTH_INVALID_CREDENTIALS` — email/password wrong or user not Admin/Moderator\n- `AUTH_CONFIG_MISSING` — JWT signing key not configured (server-side)";
            s.Responses[200] = "Login successful";
            s.Responses[400] = "Invalid credentials or insufficient permissions";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        Response = ApiResponse<Response>.SuccessResult(result, "Admin login successful");
    }
}

using FastEndpoints;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Login.AdminLogin;

public class Endpoint : Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Post("/api/users/admin/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        try
        {
            Response = await req.ExecuteAsync(ct);
        }
        catch (UnauthorizedAccessException)
        {
            await Send.ResponseAsync(ApiResponse<Response>.FailureResult("AUTH_INVALID_CREDENTIALS", "Email, password incorrect, or insufficient permissions."), 401, ct);
        }
    }
}

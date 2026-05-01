using FastEndpoints;

using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.RegisterUser.Verify;

public class Endpoint : Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Post("/api/users/verify-email");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        try
        {
            Response = await req.ExecuteAsync(ct);
        }
        catch (InvalidOperationException ex)
        {
            await SendAsync(ApiResponse<Response>.FailureResult("VERIFY_FAILED", ex.Message), 400, ct);
        }
    }
}

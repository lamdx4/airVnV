using FastEndpoints;

using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.GoogleAuth.Execute;

public class Endpoint : Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Post("/api/users/google-auth");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        try
        {
            Response = await req.ExecuteAsync(ct);
        }
        catch (Exception)
        {
            await SendAsync(ApiResponse<Response>.FailureResult("AUTH_GOOGLE_FAILED", "Xác thực Google thất bại"), 401, ct);
        }
    }
}

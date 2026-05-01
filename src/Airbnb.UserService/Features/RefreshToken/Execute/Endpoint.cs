using FastEndpoints;

using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.RefreshToken.Execute;

public class Endpoint : Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Post("/api/users/refresh-token");
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
            await SendAsync(ApiResponse<Response>.FailureResult("AUTH_TOKEN_INVALID", "Refresh token không hợp lệ hoặc đã hết hạn"), 401, ct);
        }
    }
}

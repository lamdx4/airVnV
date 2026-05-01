using FastEndpoints;

using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Login.Login;

public class Endpoint : Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Post("/api/users/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        try 
        {
            // Rule 2: Chỉ gọi ExecuteAsync()
            Response = await req.ExecuteAsync(ct);
        }
        catch (UnauthorizedAccessException)
        {
            await SendAsync(ApiResponse<Response>.FailureResult("AUTH_INVALID_CREDENTIALS", "Email hoặc mật khẩu không chính xác"), 401, ct);
        }
    }
}

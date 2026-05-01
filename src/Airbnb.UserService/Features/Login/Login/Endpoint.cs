using FastEndpoints;

namespace Airbnb.UserService.Features.Login.Login;

public class Endpoint : Endpoint<Request, Response>
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
            await SendAsync(null!, 401, ct);
        }
    }
}

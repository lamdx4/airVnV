using FastEndpoints;

namespace Airbnb.UserService.Features.RefreshToken.Execute;

public class Endpoint : Endpoint<Request, Response>
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
            await SendAsync(null!, 401, ct);
        }
    }
}

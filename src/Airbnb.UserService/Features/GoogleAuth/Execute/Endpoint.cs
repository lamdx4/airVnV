using FastEndpoints;

namespace Airbnb.UserService.Features.GoogleAuth.Execute;

public class Endpoint : Endpoint<Request, Response>
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
            await SendAsync(null!, 401, ct);
        }
    }
}

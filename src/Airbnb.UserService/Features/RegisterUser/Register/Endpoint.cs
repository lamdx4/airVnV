using FastEndpoints;

namespace Airbnb.UserService.Features.RegisterUser.Register;

public class Endpoint : Endpoint<Request, Response>
{
    public override void Configure()
    {
        Post("/api/users/register");
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
            await SendAsync(new Response(ex.Message), 400, ct);
        }
    }
}

using FastEndpoints;
using System.Security.Claims;

namespace Airbnb.UserService.Features.Profile.Update;

public class Endpoint : Endpoint<Request, Response>
{
    public override void Configure()
    {
        Put("/api/users/me");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var userIdClaim = User.FindFirstValue("UserId");
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            await SendAsync(null!, 401, ct);
            return;
        }

        // Map UserId từ Claim vào Command
        req.UserId = userId;

        // Bắt buộc dùng ExecuteAsync() cho Command (Rule 2)
        Response = await req.ExecuteAsync(ct);
    }
}

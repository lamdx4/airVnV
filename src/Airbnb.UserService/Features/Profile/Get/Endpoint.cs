using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Airbnb.UserService.Infrastructure;
using System.Security.Claims;

namespace Airbnb.UserService.Features.Profile.Get;

public class Endpoint(UserDbContext _db) : EndpointWithoutRequest<Response>
{
    public override void Configure()
    {
        Get("/api/users/me");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userIdClaim = User.FindFirstValue("UserId");
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            await SendAsync(null!, 401, ct);
            return;
        }

        // Mode 1: Simple Query - Viết trực tiếp LINQ + Projection
        var response = await _db.Users
            .Include(u => u.Profile)
            .Where(u => u.Id == userId)
            .Select(u => new Response(
                u.Id,
                u.Email,
                u.Profile.FullName,
                u.Profile.AvatarUrl,
                u.Profile.PhoneNumber,
                u.Profile.Bio,
                u.Role
            ))
            .FirstOrDefaultAsync(ct);

        if (response == null)
        {
            await SendAsync(null!, 404, ct);
            return;
        }

        Response = response;
    }
}

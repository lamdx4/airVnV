using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Airbnb.UserService.Infrastructure;

using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.RefreshToken.Execute;

public class Handler(UserDbContext _db, IConfiguration _config) : ICommandHandler<Request, ApiResponse<Response>>
{
    public async Task<ApiResponse<Response>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var user = await _db.Users
            .Include(u => u.RefreshTokens)
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == req.RefreshToken && t.RevokedAt == null && t.ExpiresAt > DateTime.UtcNow), ct);

        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid refresh token.");
        }

        var oldToken = user.RefreshTokens.First(t => t.Token == req.RefreshToken);
        oldToken.Revoke();

        var key = _config["Jwt:SigningKey"] ?? throw new InvalidOperationException("JWT Signing Key is missing.");
        var accessToken = JwtBearer.CreateToken(o =>
        {
            o.SigningKey = key;
            o.ExpireAt = DateTime.UtcNow.AddMinutes(15);
            o.User.Claims.Add(new Claim("UserId", user.Id.ToString()));
            o.User.Claims.Add(new Claim(ClaimTypes.Role, user.Role.ToString()));
        });

        var newRefreshToken = Guid.NewGuid().ToString("N");
        user.AddRefreshToken(newRefreshToken, DateTime.UtcNow.AddDays(7));

        await _db.SaveChangesAsync(ct);

        return ApiResponse<Response>.SuccessResult(new Response(accessToken, newRefreshToken), "Token refreshed successfully");
    }
}

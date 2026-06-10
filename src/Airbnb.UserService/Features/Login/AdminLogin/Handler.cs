using FastEndpoints.Security;
using Mediator;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Airbnb.UserService.Infrastructure;
using Airbnb.UserService.Domain;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Login.AdminLogin;

public sealed class Handler(UserDbContext db, IConfiguration config)
    : ICommandHandler<Request, Response>
{
    public async ValueTask<Response> Handle(Request req, CancellationToken ct)
    {
        var user = await db.Users
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Email == req.Email, ct);

        if (user is null || !BCrypt.Net.BCrypt.Verify(req.Password, user.HashedPassword ?? string.Empty))
            throw new BusinessException("Email, password incorrect, or insufficient permissions.", "AUTH_INVALID_CREDENTIALS");

        if (user.Role is not (UserRole.Admin or UserRole.Moderator))
            throw new BusinessException("Email, password incorrect, or insufficient permissions.", "AUTH_INVALID_CREDENTIALS");

        var key = config["Jwt:SigningKey"]
            ?? throw new BusinessException("JWT signing key is not configured.", "AUTH_CONFIG_MISSING");

        var accessToken = JwtBearer.CreateToken(o =>
        {
            o.SigningKey = key;
            o.ExpireAt = DateTime.UtcNow.AddMinutes(15);
            o.User.Claims.Add(new Claim("UserId", user.Id.ToString()));
            o.User.Claims.Add(new Claim("role", user.Role.ToString()));
        });

        var refreshToken = Guid.CreateVersion7().ToString("N");
        user.AddRefreshToken(refreshToken, DateTime.UtcNow.AddDays(7));
        await db.SaveChangesAsync(ct);

        return new Response(
            accessToken,
            refreshToken,
            user.Profile.FullName,
            user.Email,
            user.Role
        );
    }
}

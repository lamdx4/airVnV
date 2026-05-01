using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using FirebaseAdmin.Auth;
using Airbnb.UserService.Infrastructure;
using Airbnb.UserService.Domain;

namespace Airbnb.UserService.Features.GoogleAuth.Execute;

public class Handler(UserDbContext _db, IConfiguration _config) : ICommandHandler<Request, Response>
{
    public async Task<Response> ExecuteAsync(Request req, CancellationToken ct)
    {
        FirebaseToken decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(req.IdToken, ct);
        string email = decodedToken.Claims["email"].ToString()!;
        string fullName = decodedToken.Claims["name"]?.ToString() ?? "Google User";
        string googleId = decodedToken.Uid;

        var user = await _db.Users
            .Include(u => u.Profile)
            .Include(u => u.Logins)
            .FirstOrDefaultAsync(u => u.Email == email, ct);

        if (user == null)
        {
            user = new User(email, UserRole.User, fullName, AuthProvider.Google, googleId);
            _db.Users.Add(user);
        }
        else if (!user.Logins.Any(l => l.Provider == AuthProvider.Google && l.ProviderKey == googleId))
        {
            user.AddLogin(AuthProvider.Google, googleId);
        }

        var key = _config["Jwt:SigningKey"] ?? throw new InvalidOperationException("JWT Signing Key is missing.");
        var accessToken = JwtBearer.CreateToken(o =>
        {
            o.SigningKey = key;
            o.ExpireAt = DateTime.UtcNow.AddMinutes(15);
            o.User.Claims.Add(new Claim("UserId", user.Id.ToString()));
            o.User.Claims.Add(new Claim(ClaimTypes.Role, user.Role.ToString()));
        });

        var refreshToken = Guid.NewGuid().ToString("N");
        user.AddRefreshToken(refreshToken, DateTime.UtcNow.AddDays(7));

        await _db.SaveChangesAsync(ct);

        return new Response(accessToken, refreshToken, user.Profile.FullName, user.Email, user.Role);
    }
}

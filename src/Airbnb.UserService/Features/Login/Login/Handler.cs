using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Airbnb.UserService.Infrastructure;
using Airbnb.UserService.Domain;

namespace Airbnb.UserService.Features.Login.Login;

public class Handler(UserDbContext _db, IConfiguration _config) : ICommandHandler<Request, Response>
{
    public async Task<Response> ExecuteAsync(Request req, CancellationToken ct)
    {
        var user = await _db.Users
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Email == req.Email, ct);

        if (user == null || user.HashedPassword != req.Password)
        {
            // Trong Handler ta có thể quăng lỗi để Endpoint xử lý hoặc trả về null
            throw new UnauthorizedAccessException("Invalid credentials");
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
        
        // Domain Logic
        user.AddRefreshToken(refreshToken, DateTime.UtcNow.AddDays(7));
        
        await _db.SaveChangesAsync(ct);

        return new Response(
            accessToken, 
            refreshToken, 
            user.Profile.FullName, 
            user.Email, 
            user.Role
        );
    }
}

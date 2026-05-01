using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Airbnb.UserService.Infrastructure;
using Airbnb.UserService.Domain;

using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Login.Login;

public class Handler(UserDbContext _db, IConfiguration _config) : ICommandHandler<Request, ApiResponse<Response>>
{
    public async Task<ApiResponse<Response>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var user = await _db.Users
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Email == req.Email, ct);

        if (user == null || user.HashedPassword != req.Password)
        {
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
        
        user.AddRefreshToken(refreshToken, DateTime.UtcNow.AddDays(7));
        
        await _db.SaveChangesAsync(ct);

        return ApiResponse<Response>.SuccessResult(new Response(
            accessToken, 
            refreshToken, 
            user.Profile.FullName, 
            user.Email, 
            user.Role
        ), "Login successful");
    }
}

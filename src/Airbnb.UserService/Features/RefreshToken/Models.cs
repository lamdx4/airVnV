using FastEndpoints;
using FastEndpoints.Security;
using FluentValidation;
using Airbnb.UserService.Domain;
using Airbnb.UserService.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Airbnb.UserService.Features.RefreshToken;

public record Request(string RefreshToken);
public record Response(string AccessToken, string RefreshToken);

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}

public class Endpoint : FastEndpoints.Endpoint<Request, Response>
{
    private readonly UserDbContext _db;
    private readonly IConfiguration _config;

    public Endpoint(UserDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    public override void Configure()
    {
        Post("/api/users/refresh-token");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var tokenRecord = await _db.UserRefreshTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Token == req.RefreshToken, ct);

        if (tokenRecord is not { IsActive: true })
        {
            await SendAsync(null!, 401, ct);
            return;
        }

        // Thu hồi Refresh Token hiện tại (Rotate)
        tokenRecord.Revoke();

        var user = tokenRecord.User;
        var key = _config["Jwt:SigningKey"] ?? throw new InvalidOperationException("JWT Signing Key is missing from configuration.");
        
        var newAccessToken = JwtBearer.CreateToken(o =>
        {
            o.SigningKey = key;
            o.ExpireAt = DateTime.UtcNow.AddMinutes(15);
            o.User.Claims.Add(new Claim("UserId", user.Id.ToString()));
            o.User.Claims.Add(new Claim(ClaimTypes.Role, user.Role.ToString()));
        });

        var newRefreshToken = Guid.NewGuid().ToString("N");
        user.AddRefreshToken(newRefreshToken, DateTime.UtcNow.AddDays(7));
        
        await _db.SaveChangesAsync(ct);

        Response = new Response(newAccessToken, newRefreshToken);
    }
}

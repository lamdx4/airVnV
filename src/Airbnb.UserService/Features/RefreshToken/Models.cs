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
    private readonly UserDbContext db;
    private readonly IConfiguration config;

    public Endpoint(UserDbContext db, IConfiguration config)
    {
        this.db = db;
        this.config = config;
    }

    public override void Configure()
    {
        Post("/api/users/refresh-token");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var tokenRecord = await db.UserRefreshTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Token == req.RefreshToken, ct);

        if (tokenRecord == null || !tokenRecord.IsActive)
        {
            await SendAsync(null!, 401, ct);
            return;
        }

        // Thu hồi Refresh Token hiện tại (Rotate)
        tokenRecord.Revoke();

        var user = tokenRecord.User;
        var key = config["Jwt:SigningKey"] ?? "SuperSecretKeyThatIsAtLeast32CharsLong!!";
        
        var newAccessToken = JWTBearer.CreateToken(
            signingKey: key,
            expireAt: DateTime.UtcNow.AddMinutes(15),
            claims: [
                new Claim("UserId", user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            ]);

        var newRefreshToken = Guid.NewGuid().ToString("N");
        user.AddRefreshToken(newRefreshToken, DateTime.UtcNow.AddDays(7));
        
        await db.SaveChangesAsync(ct);

        Response = new Response(newAccessToken, newRefreshToken);
    }
}

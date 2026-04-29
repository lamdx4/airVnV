using FastEndpoints;
using FastEndpoints.Security;
using FluentValidation;
using Airbnb.UserService.Domain;
using Airbnb.UserService.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Airbnb.UserService.Features.Login;

public record Request(string Email, string Password);
public record Response(string Token, string FullName, string Email, UserRole Role);

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
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
        Post("/api/users/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var user = await db.Users
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Email == req.Email, ct);

        if (user == null || user.HashedPassword != req.Password)
        {
            await base.SendAsync(null!, 401, ct);
            return;
        }

        var key = config["Jwt:SigningKey"] ?? "SuperSecretKeyThatIsAtLeast32CharsLong!!";
        var jwtToken = JWTBearer.CreateToken(
            signingKey: key,
            expireAt: DateTime.UtcNow.AddDays(1),
            claims: [
                new Claim("UserId", user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            ]);

        Response = new Response(jwtToken, user.Profile.FullName, user.Email, user.Role);
    }
}

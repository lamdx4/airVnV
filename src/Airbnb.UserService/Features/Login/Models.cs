using FastEndpoints;
using FastEndpoints.Security;
using FluentValidation;
using Airbnb.UserService.Domain;
using Airbnb.UserService.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Airbnb.UserService.Features.Login;

public record Request(string Email, string Password);
public record Response(string AccessToken, string RefreshToken, string FullName, string Email, UserRole Role);

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
    private readonly UserDbContext _db;
    private readonly IConfiguration _config;

    public Endpoint(UserDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    public override void Configure()
    {
        Post("/api/users/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var user = await _db.Users
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Email == req.Email, ct);

        if (user == null || user.HashedPassword != req.Password)
        {
            await base.SendAsync(null!, 401, ct);
            return;
        }

        var key = _config["Jwt:SigningKey"] ?? throw new InvalidOperationException("JWT Signing Key is missing from configuration.");
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

        Response = new Response(accessToken, refreshToken, user.Profile.FullName, user.Email, user.Role);
    }
}

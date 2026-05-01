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

// Application Layer: Handler
public class LoginHandler(UserDbContext _db, IConfiguration _config)
{
    public async Task<Response?> HandleAsync(Request req, CancellationToken ct)
    {
        var user = await _db.Users
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Email == req.Email, ct);

        if (user == null || user.HashedPassword != req.Password)
        {
            return null;
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

        return new Response(accessToken, refreshToken, user.Profile.FullName, user.Email, user.Role);
    }
}

// Web Layer: Endpoint
public class Endpoint(LoginHandler _handler) : FastEndpoints.Endpoint<Request, Response>
{
    public override void Configure()
    {
        Post("/api/users/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await _handler.HandleAsync(req, ct);

        if (result == null)
        {
            await base.SendAsync(null!, 401, ct);
            return;
        }

        Response = result;
    }
}

using FastEndpoints;
using FastEndpoints.Security;
using FluentValidation;
using Airbnb.UserService.Domain;
using Airbnb.UserService.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Google.Apis.Auth;
using System.Security.Claims;

namespace Airbnb.UserService.Features.GoogleAuth;

public record Request(string IdToken, UserRole Role);
public record Response(string AccessToken, string RefreshToken, string FullName, string Email, UserRole Role);

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.IdToken).NotEmpty();
        RuleFor(x => x.Role).IsInEnum();
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
        Post("/api/users/google-auth");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        GoogleJsonWebSignature.Payload payload;
        try
        {
            payload = await GoogleJsonWebSignature.ValidateAsync(req.IdToken);
        }
        catch (Exception)
        {
            await SendAsync(null!, 401, ct);
            return;
        }

        var googleId = payload.Subject;
        var email = payload.Email;
        var fullName = payload.Name ?? "Guest User";

        string token;
        string refreshToken;

        // 1. Kịch bản 1: Đăng nhập bằng Google đã có liên kết
        var login = await db.UserLogins
            .Include(l => l.User)
            .ThenInclude(u => u.Profile)
            .FirstOrDefaultAsync(l => l.Provider == AuthProvider.Google && l.ProviderKey == googleId, ct);

        User? user = login?.User;

        if (user != null)
        {
            token = GenerateJwt(user);
            refreshToken = Guid.NewGuid().ToString("N");
            user.AddRefreshToken(refreshToken, DateTime.UtcNow.AddDays(7));
            await db.SaveChangesAsync(ct);

            Response = new Response(token, refreshToken, user.Profile.FullName, user.Email, user.Role);
            return;
        }

        // 2. Kịch bản 2: Liên kết tài khoản qua Email
        user = await db.Users
            .Include(u => u.Logins)
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Email == email, ct);

        if (user != null)
        {
            user.AddLogin(AuthProvider.Google, googleId);
            token = GenerateJwt(user);
            refreshToken = Guid.NewGuid().ToString("N");
            user.AddRefreshToken(refreshToken, DateTime.UtcNow.AddDays(7));
            await db.SaveChangesAsync(ct);

            Response = new Response(token, refreshToken, user.Profile.FullName, user.Email, user.Role);
            return;
        }

        // 3. Kịch bản 3: Đăng ký mới
        user = new User(email, req.Role, fullName, AuthProvider.Google, googleId);

        db.Users.Add(user);
        token = GenerateJwt(user);
        refreshToken = Guid.NewGuid().ToString("N");
        user.AddRefreshToken(refreshToken, DateTime.UtcNow.AddDays(7));
        await db.SaveChangesAsync(ct);

        Response = new Response(token, refreshToken, user.Profile.FullName, user.Email, user.Role);
    }

    private string GenerateJwt(User user)
    {
        var key = config["Jwt:SigningKey"] ?? throw new InvalidOperationException("JWT Signing Key is missing from configuration.");
        return JWTBearer.CreateToken(
            signingKey: key,
            expireAt: DateTime.UtcNow.AddMinutes(15),
            claims: [
                new Claim("UserId", user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            ]);
    }
}

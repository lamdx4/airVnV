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
public record Response(string Token, string FullName, string Email, UserRole Role);

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
    public Endpoint(UserDbContext db) => this.db = db;

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

        // 1. Kịch bản 1: Đăng nhập bằng Google đã có liên kết
        var login = await db.UserLogins
            .Include(l => l.User)
            .ThenInclude(u => u.Profile)
            .FirstOrDefaultAsync(l => l.Provider == AuthProvider.Google && l.ProviderKey == googleId, ct);

        User? user = login?.User;

        if (user != null)
        {
            var token = GenerateJwt(user);
            Response = new Response(token, user.Profile.FullName, user.Email, user.Role);
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
            await db.SaveChangesAsync(ct);

            var token = GenerateJwt(user);
            Response = new Response(token, user.Profile.FullName, user.Email, user.Role);
            return;
        }

        // 3. Kịch bản 3: Đăng ký mới
        user = new User(email, req.Role, fullName, AuthProvider.Google, googleId);

        db.Users.Add(user);
        await db.SaveChangesAsync(ct);

        var jwtToken = GenerateJwt(user);
        Response = new Response(jwtToken, user.Profile.FullName, user.Email, user.Role);
    }

    private string GenerateJwt(User user)
    {
        return JWTBearer.CreateToken(
            signingKey: "SuperSecretKeyThatIsAtLeast32CharsLong!!",
            expireAt: DateTime.UtcNow.AddDays(1),
            claims: [
                new Claim("UserId", user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            ]);
    }
}

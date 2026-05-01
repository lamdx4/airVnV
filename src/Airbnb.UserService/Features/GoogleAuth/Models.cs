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

// Application Layer: Handler
public class GoogleAuthHandler(UserDbContext _db, IConfiguration _config)
{
    public async Task<Response?> HandleAsync(Request req, CancellationToken ct)
    {
        GoogleJsonWebSignature.Payload payload;
        try
        {
            payload = await GoogleJsonWebSignature.ValidateAsync(req.IdToken);
        }
        catch (Exception)
        {
            return null;
        }

        var googleId = payload.Subject;
        var email = payload.Email;
        var fullName = payload.Name ?? "Guest User";

        // 1. Kịch bản 1: Đăng nhập bằng Google đã có liên kết
        var login = await _db.UserLogins
            .Include(l => l.User)
            .ThenInclude(u => u.Profile)
            .FirstOrDefaultAsync(l => l.Provider == AuthProvider.Google && l.ProviderKey == googleId, ct);

        User? user = login?.User;

        if (user != null)
        {
            return await CreateAuthResponse(user, ct);
        }

        // 2. Kịch bản 2: Liên kết tài khoản qua Email
        user = await _db.Users
            .Include(u => u.Logins)
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Email == email, ct);

        if (user != null)
        {
            user.AddLogin(AuthProvider.Google, googleId);
            return await CreateAuthResponse(user, ct);
        }

        // 3. Kịch bản 3: Đăng ký mới
        user = new User(email, req.Role, fullName, AuthProvider.Google, googleId);
        _db.Users.Add(user);
        return await CreateAuthResponse(user, ct);
    }

    private async Task<Response> CreateAuthResponse(User user, CancellationToken ct)
    {
        var key = _config["Jwt:SigningKey"] ?? throw new InvalidOperationException("JWT Signing Key is missing from configuration.");
        var token = JwtBearer.CreateToken(o =>
        {
            o.SigningKey = key;
            o.ExpireAt = DateTime.UtcNow.AddMinutes(15);
            o.User.Claims.Add(new Claim("UserId", user.Id.ToString()));
            o.User.Claims.Add(new Claim(ClaimTypes.Role, user.Role.ToString()));
        });

        var refreshToken = Guid.NewGuid().ToString("N");
        user.AddRefreshToken(refreshToken, DateTime.UtcNow.AddDays(7));
        await _db.SaveChangesAsync(ct);

        return new Response(token, refreshToken, user.Profile.FullName, user.Email, user.Role);
    }
}

// Web Layer: Endpoint
public class Endpoint(GoogleAuthHandler _handler) : Endpoint<Request, Response>
{
    public override void Configure()
    {
        Post("/api/users/google-auth");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await _handler.HandleAsync(req, ct);

        if (result == null)
        {
            await SendAsync(null!, 401, ct);
            return;
        }

        Response = result;
    }
}

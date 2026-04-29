using FastEndpoints;
using FastEndpoints.Security;
using FluentValidation;
using Airbnb.UserService.Domain;
using Airbnb.UserService.Infrastructure;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace Airbnb.UserService.Features.RegisterUser;

public record VerifyEmailRequest(string Email, string OtpCode);
public record VerifyEmailResponse(string AccessToken, string RefreshToken, string FullName, string Email, UserRole Role);

public class VerifyEmailValidator : Validator<VerifyEmailRequest>
{
    public VerifyEmailValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.OtpCode).NotEmpty().Length(6);
    }
}

public class VerifyEmailEndpoint : Endpoint<VerifyEmailRequest, VerifyEmailResponse>
{
    private readonly UserDbContext db;
    private readonly IMemoryCache cache;
    private readonly IConfiguration config;

    public VerifyEmailEndpoint(UserDbContext db, IMemoryCache cache, IConfiguration config)
    {
        this.db = db;
        this.cache = cache;
        this.config = config;
    }

    public override void Configure()
    {
        Post("/api/users/verify-email");
        AllowAnonymous();
    }

    public override async Task HandleAsync(VerifyEmailRequest req, CancellationToken ct)
    {
        var cacheKey = $"reg_{req.Email}";
        
        if (!cache.TryGetValue(cacheKey, out (Request regData, string otp) cached))
        {
            await SendAsync(null!, 400, ct);
            return;
        }

        if (cached.otp != req.OtpCode)
        {
            await SendAsync(null!, 400, ct);
            return;
        }

        // OTP Đúng -> Tiến hành tạo tài khoản vào Postgres
        var user = new User(cached.regData.Email, cached.regData.Password, cached.regData.Role, cached.regData.FullName);
        db.Users.Add(user);

        // Tạo Tokens
        var key = config["Jwt:SigningKey"] ?? throw new InvalidOperationException("JWT Signing Key is missing from configuration.");
        
        var accessToken = JWTBearer.CreateToken(
            signingKey: key,
            expireAt: DateTime.UtcNow.AddMinutes(15),
            claims: [
                new Claim("UserId", user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            ]);

        var refreshToken = Guid.NewGuid().ToString("N");
        user.AddRefreshToken(refreshToken, DateTime.UtcNow.AddDays(7));
        
        await db.SaveChangesAsync(ct);

        // Dọn dẹp Cache
        cache.Remove(cacheKey);

        Response = new VerifyEmailResponse(accessToken, refreshToken, user.Profile.FullName, user.Email, user.Role);
    }
}

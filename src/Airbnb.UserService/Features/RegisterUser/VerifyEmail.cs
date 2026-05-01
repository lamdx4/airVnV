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

// Application Layer: Handler
public class VerifyEmailHandler(UserDbContext _db, IMemoryCache _cache, IConfiguration _config)
{
    public async Task<VerifyEmailResponse?> HandleAsync(VerifyEmailRequest req, CancellationToken ct)
    {
        var cacheKey = $"reg_{req.Email}";
        
        if (!_cache.TryGetValue(cacheKey, out (Request regData, string otp, int failedAttempts) cached))
        {
            return null;
        }

        if (cached.otp != req.OtpCode)
        {
            var newFailedAttempts = cached.failedAttempts + 1;

            if (newFailedAttempts >= 5)
            {
                _cache.Remove(cacheKey);
                throw new InvalidOperationException("Too many failed attempts");
            }

            _cache.Set(cacheKey, (cached.regData, cached.otp, newFailedAttempts), TimeSpan.FromMinutes(15));
            return null;
        }

        // OTP Đúng -> Tiến hành tạo tài khoản vào Postgres
        var user = new User(cached.regData.Email, cached.regData.Password, cached.regData.Role, cached.regData.FullName);
        _db.Users.Add(user);

        // Tạo Tokens
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

        // Dọn dẹp Cache
        _cache.Remove(cacheKey);

        return new VerifyEmailResponse(accessToken, refreshToken, user.Profile.FullName, user.Email, user.Role);
    }
}

// Web Layer: Endpoint
public class VerifyEmailEndpoint(VerifyEmailHandler _handler) : Endpoint<VerifyEmailRequest, VerifyEmailResponse>
{
    public override void Configure()
    {
        Post("/api/users/verify-email");
        AllowAnonymous();
    }

    public override async Task HandleAsync(VerifyEmailRequest req, CancellationToken ct)
    {
        try
        {
            var result = await _handler.HandleAsync(req, ct);

            if (result == null)
            {
                await SendAsync(null!, 400, ct);
                return;
            }

            Response = result;
        }
        catch (InvalidOperationException ex) when (ex.Message == "Too many failed attempts")
        {
            await SendAsync(null!, 429, ct);
        }
    }
}

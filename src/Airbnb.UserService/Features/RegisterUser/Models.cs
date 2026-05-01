using FastEndpoints;
using FluentValidation;
using Airbnb.UserService.Domain;
using Airbnb.UserService.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Airbnb.UserService.Features.RegisterUser;

public record Request(string FullName, string Email, string Password, UserRole Role);
public record Response(string Message, string? OtpCode);

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
        RuleFor(x => x.Role).IsInEnum();
    }
}

// Application Layer: Handler
public class RegisterHandler(UserDbContext _db, IMemoryCache _cache)
{
    public async Task<Response?> HandleAsync(Request req, CancellationToken ct)
    {
        var exists = await _db.Users.AnyAsync(u => u.Email == req.Email, ct);
        if (exists)
        {
            return null;
        }

        // Tạo mã OTP ngẫu nhiên 6 số
        var otp = new Random().Next(100000, 999999).ToString();
        
        // Lưu thông tin payload + OTP vào MemoryCache 15 phút
        var cacheKey = $"reg_{req.Email}";
        _cache.Set(cacheKey, (req, otp, 0), TimeSpan.FromMinutes(15));

        return new Response("Mã OTP đã được tạo!", otp);
    }
}

// Web Layer: Endpoint
public class Endpoint(RegisterHandler _handler) : FastEndpoints.Endpoint<Request, Response>
{
    public override void Configure()
    {
        Post("/api/users/register");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await _handler.HandleAsync(req, ct);

        if (result == null)
        {
            await SendAsync(null!, 400, ct);
            return;
        }

        Response = result;
    }
}

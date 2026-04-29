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

public class Endpoint : FastEndpoints.Endpoint<Request, Response>
{
    private readonly UserDbContext db;
    private readonly IMemoryCache cache;

    public Endpoint(UserDbContext db, IMemoryCache cache)
    {
        this.db = db;
        this.cache = cache;
    }

    public override void Configure()
    {
        Post("/api/users/register");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var exists = await db.Users.AnyAsync(u => u.Email == req.Email, ct);
        if (exists)
        {
            await SendAsync(null!, 400, ct);
            return;
        }

        // Tạo mã OTP ngẫu nhiên 6 số
        var otp = new Random().Next(100000, 999999).ToString();
        
        // Lưu thông tin payload + OTP vào MemoryCache 15 phút
        var cacheKey = $"reg_{req.Email}";
        cache.Set(cacheKey, (req, otp, 0), TimeSpan.FromMinutes(15));

        // Trả về mã OTP (Chỉ làm ở môi trường Dev/Học tập cho tiện FE)
        Response = new Response("Mã OTP đã được tạo!", otp);
    }
}

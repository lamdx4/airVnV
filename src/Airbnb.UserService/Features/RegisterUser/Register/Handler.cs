using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Airbnb.UserService.Infrastructure;
using Airbnb.UserService.Domain;

using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.RegisterUser.Register;

public class Handler(UserDbContext _db, IMemoryCache _cache) : ICommandHandler<Request, ApiResponse<Response>>
{
    public async Task<ApiResponse<Response>> ExecuteAsync(Request req, CancellationToken ct)
    {
        // Kiểm tra email tồn tại
        if (await _db.Users.AnyAsync(u => u.Email == req.Email, ct))
        {
            throw new InvalidOperationException("Email already exists.");
        }

        // Tạo Entity qua Rich Domain Model
        var user = new User(req.Email, req.Password, UserRole.User, req.FullName);
        
        // Lưu tạm vào Cache để Verify
        var verifyCode = new Random().Next(100000, 999999).ToString();
        _cache.Set($"verify_{req.Email}", (user, verifyCode), TimeSpan.FromMinutes(15));

        return ApiResponse<Response>.SuccessResult(new Response($"Verification code sent to {req.Email}. Code: {verifyCode}"), "Registration initiated");
    }
}

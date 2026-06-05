using FastEndpoints;
using Microsoft.Extensions.Caching.Memory;
using Airbnb.UserService.Infrastructure;
using Airbnb.UserService.Domain;

using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.RegisterUser.Verify;

public class Handler(UserDbContext _db, IMemoryCache _cache) : Mediator.ICommandHandler<Request, ApiResponse<Response>>
{
    public async ValueTask<ApiResponse<Response>> Handle(Request req, CancellationToken ct)
    {
        if (!_cache.TryGetValue($"verify_{req.Email}", out (User user, string code) cached))
        {
            throw new BusinessException("Verification code expired or not found.", "VERIFY_CODE_EXPIRED");
        }

        if (cached.code != req.Code)
        {
            throw new BusinessException("Invalid verification code.", "VERIFY_INVALID_CODE");
        }

        // Lưu vào Database thực tế
        _db.Users.Add(cached.user);
        await _db.SaveChangesAsync(ct);
        
        // Dọn cache
        _cache.Remove($"verify_{req.Email}");

        return ApiResponse<Response>.SuccessResult(new Response("Email verified and account created successfully."), "Verification successful");
    }
}

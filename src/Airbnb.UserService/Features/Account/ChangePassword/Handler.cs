using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.UserService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;
using BCrypt.Net;

namespace Airbnb.UserService.Features.Account.ChangePassword;

public sealed class Handler(UserDbContext _db, IHttpContextAccessor _httpContextAccessor) 
    : ICommandHandler<Request, ApiResponse<bool>>
{
    public async ValueTask<ApiResponse<bool>> Handle(Request req, CancellationToken ct)
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("UserId")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return ApiResponse<bool>.FailureResult("UNAUTHORIZED", "Người dùng chưa đăng nhập");
        }

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);
        if (user == null)
        {
            return ApiResponse<bool>.FailureResult("NOT_FOUND", "Người dùng không tồn tại");
        }

        if (string.IsNullOrEmpty(user.HashedPassword))
        {
            return ApiResponse<bool>.FailureResult("SOCIAL_USER", "Tài khoản mạng xã hội không thể đổi mật khẩu theo cách này");
        }

        if (!BCrypt.Net.BCrypt.Verify(req.CurrentPassword, user.HashedPassword))
        {
            return ApiResponse<bool>.FailureResult("INVALID_PASSWORD", "Mật khẩu hiện tại không chính xác");
        }

        user.SetPassword(BCrypt.Net.BCrypt.HashPassword(req.NewPassword));
        
        await _db.SaveChangesAsync(ct);

        return ApiResponse<bool>.SuccessResult(true, "Đổi mật khẩu thành công");
    }
}

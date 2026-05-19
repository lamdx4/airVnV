using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.UserService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;
using System.Security.Claims;

namespace Airbnb.UserService.Features.Account.RevokeSession;

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

        var token = await _db.Users
            .Where(u => u.Id == userId)
            .SelectMany(u => u.RefreshTokens)
            .FirstOrDefaultAsync(t => t.Id == req.SessionId, ct);

        if (token == null)
        {
            return ApiResponse<bool>.FailureResult("NOT_FOUND", "Phiên đăng nhập không tồn tại");
        }

        token.Revoke();
        await _db.SaveChangesAsync(ct);

        return ApiResponse<bool>.SuccessResult(true, "Đã đăng xuất thiết bị thành công");
    }
}

using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.UserService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;
using System.Security.Claims;

namespace Airbnb.UserService.Features.Account.GetSessions;

public sealed class Handler(UserDbContext _db, IHttpContextAccessor _httpContextAccessor) 
    : IQueryHandler<Request, ApiResponse<List<SessionResponse>>>
{
    public async ValueTask<ApiResponse<List<SessionResponse>>> Handle(Request req, CancellationToken ct)
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("UserId")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return ApiResponse<List<SessionResponse>>.FailureResult("UNAUTHORIZED", "Người dùng chưa đăng nhập");
        }

        var sessions = await _db.Users
            .Where(u => u.Id == userId)
            .SelectMany(u => u.RefreshTokens)
            .Where(t => t.RevokedAt == null && t.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(t => t.LoginAt)
            .Select(t => new SessionResponse(
                t.Id,
                t.UserAgent,
                t.IpAddress,
                t.LoginAt,
                t.ExpiresAt,
                false // placeholder
            ))
            .ToListAsync(ct);

        return ApiResponse<List<SessionResponse>>.SuccessResult(sessions);
    }
}

using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Account.GetSessions;

public record Request : Mediator.IQuery<ApiResponse<List<SessionResponse>>>;

public record SessionResponse(
    Guid Id,
    string? UserAgent,
    string? IpAddress,
    DateTime LoginAt,
    DateTime ExpiresAt,
    bool IsCurrent
);

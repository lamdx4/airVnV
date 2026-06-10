using Airbnb.UserService.Domain;

namespace Airbnb.UserService.Features.Admin.GetUsers;

public record Request : Mediator.IQuery<PaginatedUserListResponse>
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? Search { get; init; }
    public string? Role { get; init; }
    public string? Status { get; init; }
    public string? SortBy { get; init; }
    public string? SortOrder { get; init; } = "asc";
}

public record UserSummaryResponse(
    Guid Id,
    string Email,
    string FullName,
    string? AvatarUrl,
    UserRole Role,
    UserStatus Status,
    bool IsVerified,
    DateTime CreatedAt,
    DateTime? LastLoginAt
);

public record PaginatedUserListResponse(
    List<UserSummaryResponse> Items,
    int TotalCount,
    int PageNumber,
    int PageSize,
    int TotalPages
);

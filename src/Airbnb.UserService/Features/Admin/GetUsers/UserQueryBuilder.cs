using Airbnb.UserService.Domain;

namespace Airbnb.UserService.Features.Admin.GetUsers;

/// <summary>
/// Builder Pattern: gom các bước xây dựng query phức tạp (filter + sort)
/// cho danh sách User admin thành một fluent API có thể compose.
/// Tách hẳn việc "xây dựng query" khỏi handler — handler chỉ orchestration.
/// </summary>
public sealed class UserQueryBuilder
{
    private IQueryable<User> _query;

    public UserQueryBuilder(IQueryable<User> source)
    {
        _query = source;
    }

    public UserQueryBuilder WithSearch(string? search)
    {
        if (string.IsNullOrWhiteSpace(search)) return this;

        var needle = search.ToLower();
        _query = _query.Where(u =>
            u.Email.ToLower().Contains(needle) ||
            u.Profile.FullName.ToLower().Contains(needle));
        return this;
    }

    public UserQueryBuilder WithRole(string? role)
    {
        if (string.IsNullOrWhiteSpace(role)) return this;
        if (!Enum.TryParse<UserRole>(role, true, out var parsed)) return this;

        _query = _query.Where(u => u.Role == parsed);
        return this;
    }

    public UserQueryBuilder WithStatus(string? status)
    {
        if (string.IsNullOrWhiteSpace(status)) return this;
        if (!Enum.TryParse<UserStatus>(status, true, out var parsed)) return this;

        _query = _query.Where(u => u.Status == parsed);
        return this;
    }

    public UserQueryBuilder OrderBy(string? sortBy, string? sortOrder)
    {
        var field = (sortBy ?? "CreatedAt").ToLowerInvariant();
        var descending = string.Equals(sortOrder, "desc", StringComparison.OrdinalIgnoreCase);

        _query = field switch
        {
            "email"       => descending ? _query.OrderByDescending(u => u.Email)             : _query.OrderBy(u => u.Email),
            "fullname"    => descending ? _query.OrderByDescending(u => u.Profile.FullName)  : _query.OrderBy(u => u.Profile.FullName),
            "role"        => descending ? _query.OrderByDescending(u => u.Role)              : _query.OrderBy(u => u.Role),
            "status"      => descending ? _query.OrderByDescending(u => u.Status)            : _query.OrderBy(u => u.Status),
            "lastloginat" => descending ? _query.OrderByDescending(u => u.LastLoginAt)       : _query.OrderBy(u => u.LastLoginAt),
            _             => descending ? _query.OrderByDescending(u => u.CreatedAt)         : _query.OrderBy(u => u.CreatedAt),
        };
        return this;
    }

    /// <summary>Trả về IQueryable cuối — chưa kèm pagination, để caller tự Count + Skip/Take.</summary>
    public IQueryable<User> Build() => _query;
}

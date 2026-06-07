using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Airbnb.UserService.Infrastructure;
using Airbnb.UserService.Domain;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Admin.GetUsers;

public sealed class GetUsersHandler(UserDbContext db)
    : ICommandHandler<Request, ApiResponse<PaginatedUserListResponse>>
{
    public async Task<ApiResponse<PaginatedUserListResponse>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var query = db.Users
            .AsNoTracking()
            .Include(u => u.Profile)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(req.Search))
        {
            var search = req.Search.ToLower();
            query = query.Where(u =>
                u.Email.ToLower().Contains(search) ||
                u.Profile.FullName.ToLower().Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(req.Role) && Enum.TryParse<UserRole>(req.Role, true, out var role))
        {
            query = query.Where(u => u.Role == role);
        }

        if (!string.IsNullOrWhiteSpace(req.Status) && Enum.TryParse<UserStatus>(req.Status, true, out var status))
        {
            query = query.Where(u => u.Status == status);
        }

        var sortBy = req.SortBy ?? "CreatedAt";
        var sortOrder = req.SortOrder ?? "asc";

        query = sortBy.ToLower() switch
        {
            "email" => sortOrder == "desc" ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email),
            "fullname" => sortOrder == "desc" ? query.OrderByDescending(u => u.Profile.FullName) : query.OrderBy(u => u.Profile.FullName),
            "role" => sortOrder == "desc" ? query.OrderByDescending(u => u.Role) : query.OrderBy(u => u.Role),
            "status" => sortOrder == "desc" ? query.OrderByDescending(u => u.Status) : query.OrderBy(u => u.Status),
            "createdat" => sortOrder == "desc" ? query.OrderByDescending(u => u.CreatedAt) : query.OrderBy(u => u.CreatedAt),
            "lastloginat" => sortOrder == "desc" ? query.OrderByDescending(u => u.LastLoginAt) : query.OrderBy(u => u.LastLoginAt),
            _ => sortOrder == "desc" ? query.OrderByDescending(u => u.CreatedAt) : query.OrderBy(u => u.CreatedAt),
        };

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((req.Page - 1) * req.PageSize)
            .Take(req.PageSize)
            .Select(u => new UserSummaryResponse(
                u.Id,
                u.Email,
                u.Profile.FullName,
                u.Profile.AvatarUrl,
                u.Role,
                u.Status,
                u.IsVerified,
                u.CreatedAt,
                u.LastLoginAt
            ))
            .ToListAsync(ct);

        var totalPages = (int)Math.Ceiling(totalCount / (double)req.PageSize);

        return ApiResponse<PaginatedUserListResponse>.SuccessResult(
            new PaginatedUserListResponse(items, totalCount, req.Page, req.PageSize, totalPages),
            "Users retrieved successfully"
        );
    }
}

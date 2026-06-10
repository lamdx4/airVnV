using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.UserService.Infrastructure;

namespace Airbnb.UserService.Features.Admin.GetUsers;

public sealed class GetUsersHandler(UserDbContext db)
    : IQueryHandler<Request, PaginatedUserListResponse>
{
    public async ValueTask<PaginatedUserListResponse> Handle(Request req, CancellationToken ct)
    {
        var query = new UserQueryBuilder(
                db.Users.AsNoTracking().Include(u => u.Profile))
            .WithSearch(req.Search)
            .WithRole(req.Role)
            .WithStatus(req.Status)
            .OrderBy(req.SortBy, req.SortOrder)
            .Build();

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

        return new PaginatedUserListResponse(items, totalCount, req.Page, req.PageSize, totalPages);
    }
}

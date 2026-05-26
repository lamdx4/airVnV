using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Admin.UserManagement.GetUsers;

public record Request(
    int Page = 1,
    int PageSize = 20,
    string? Search = null,
    string? Role = null,
    string? Status = null,
    string? SortBy = null,
    string SortOrder = "desc"
) : Mediator.IQuery<Response>;

public record Response(
    List<UserListItem> Items,
    int TotalCount,
    int Page,
    int PageSize
);

public record UserListItem(
    Guid Id,
    string Email,
    string FullName,
    string Role,
    string Status,
    string? AvatarUrl,
    bool IsVerified,
    string KycStatus,
    DateTime CreatedAt
);

public sealed class Handler(Airbnb.UserService.Infrastructure.UserDbContext db) : IQueryHandler<Request, Response>
{
    public async ValueTask<Response> Handle(Request req, CancellationToken ct)
    {
        var query = db.Users
            .Include(u => u.Profile)
            .AsNoTracking();

        // Search filter
        if (!string.IsNullOrWhiteSpace(req.Search))
        {
            var search = req.Search.ToLower();
            query = query.Where(u =>
                u.Email.ToLower().Contains(search) ||
                u.Profile.FullName.ToLower().Contains(search));
        }

        // Role filter
        if (!string.IsNullOrWhiteSpace(req.Role) &&
            Enum.TryParse<Domain.UserRole>(req.Role, true, out var role))
        {
            query = query.Where(u => u.Role == role);
        }

        // Status filter
        if (!string.IsNullOrWhiteSpace(req.Status) &&
            Enum.TryParse<Domain.UserStatus>(req.Status, true, out var status))
        {
            query = query.Where(u => u.Status == status);
        }

        // Sorting
        query = req.SortBy?.ToLower() switch
        {
            "email" => req.SortOrder == "asc"
                ? query.OrderBy(u => u.Email)
                : query.OrderByDescending(u => u.Email),
            "createdat" => req.SortOrder == "asc"
                ? query.OrderBy(u => u.CreatedAt)
                : query.OrderByDescending(u => u.CreatedAt),
            "fullname" => req.SortOrder == "asc"
                ? query.OrderBy(u => u.Profile.FullName)
                : query.OrderByDescending(u => u.Profile.FullName),
            _ => query.OrderByDescending(u => u.CreatedAt),
        };

        var totalCount = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.CountAsync(query, ct);

        var items = await query
            .Skip((req.Page - 1) * req.PageSize)
            .Take(req.PageSize)
            .Select(u => new UserListItem(
                u.Id,
                u.Email,
                u.Profile.FullName,
                u.Role.ToString(),
                u.Status.ToString(),
                u.Profile.AvatarUrl,
                u.KycStatus == Domain.KycStatus.Approved,
                u.KycStatus.ToString(),
                u.CreatedAt
            ))
            .ToListAsync(ct);

        return new Response(items, totalCount, req.Page, req.PageSize);
    }
}

public sealed class Endpoint(IMediator mediator) : FastEndpoints.Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Get("/api/admin/users");
        Policies("AdminOnly");
        Summary(s => {
            s.Summary = "Admin get users list with filtering and pagination";
            s.Description = "Possible Error Codes: \n" +
                            "- **UNAUTHORIZED**: Not authorized to access this endpoint.";
            s.Responses[200] = "Users list retrieved successfully.";
            s.Responses[401] = "Unauthorized - Admin role required.";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await Send.ResponseAsync(ApiResponse<Response>.SuccessResult(result), cancellation: ct);
    }
}
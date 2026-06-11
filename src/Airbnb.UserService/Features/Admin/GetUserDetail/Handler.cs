using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.UserService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Admin.GetUserDetail;

public sealed class GetUserDetailHandler(UserDbContext db)
    : IQueryHandler<GetUserDetailRequest, UserDetailResponse>
{
    public async ValueTask<UserDetailResponse> Handle(GetUserDetailRequest req, CancellationToken ct)
    {
        var user = await db.Users
            .AsNoTracking()
            .Include(u => u.Profile)
            .Where(u => u.Id == req.Id)
            .Select(u => new UserDetailResponse(
                u.Id,
                u.Email,
                u.Profile.FullName,
                u.Profile.AvatarUrl,
                u.Profile.PhoneNumber,
                u.Profile.Bio,
                u.Role,
                u.Status,
                u.IsVerified,
                u.CreatedAt,
                u.LastLoginAt,
                u.SuspensionReason,
                u.BanReason
            ))
            .FirstOrDefaultAsync(ct);

        return user ?? throw new NotFoundException("User not found.");
    }
}

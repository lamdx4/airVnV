using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Airbnb.UserService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Admin.SuspendUser;

public sealed class SuspendUserHandler(UserDbContext db)
    : ICommandHandler<Request, ApiResponse<UserActionResponse>>
{
    public async Task<ApiResponse<UserActionResponse>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var user = await db.Users.FindAsync([req.Id], ct);
        if (user is null)
            throw new NotFoundException("User not found.");

        user.Suspend(req.Reason);
        await db.SaveChangesAsync(ct);

        return ApiResponse<UserActionResponse>.SuccessResult(
            new UserActionResponse(user.Id, user.Status.ToString(), "User suspended successfully"),
            "User suspended"
        );
    }
}

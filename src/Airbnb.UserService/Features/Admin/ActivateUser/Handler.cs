using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Airbnb.UserService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Admin.ActivateUser;

public sealed class ActivateUserHandler(UserDbContext db)
    : ICommandHandler<ActivateUserRequest, ApiResponse<ActivateUserResponse>>
{
    public async Task<ApiResponse<ActivateUserResponse>> ExecuteAsync(ActivateUserRequest req, CancellationToken ct)
    {
        var user = await db.Users.FindAsync([req.Id], ct);
        if (user is null)
            throw new NotFoundException("User not found.");

        user.Activate();
        await db.SaveChangesAsync(ct);

        return ApiResponse<ActivateUserResponse>.SuccessResult(
            new ActivateUserResponse(user.Id, user.Status.ToString(), "User activated successfully"),
            "User activated"
        );
    }
}

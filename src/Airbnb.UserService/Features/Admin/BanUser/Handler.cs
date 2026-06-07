using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Airbnb.UserService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Admin.BanUser;

public sealed class BanUserHandler(UserDbContext db)
    : ICommandHandler<Request, ApiResponse<BanUserResponse>>
{
    public async Task<ApiResponse<BanUserResponse>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var user = await db.Users.FindAsync([req.Id], ct);
        if (user is null)
            throw new NotFoundException("User not found.");

        user.Ban(req.Reason);
        await db.SaveChangesAsync(ct);

        return ApiResponse<BanUserResponse>.SuccessResult(
            new BanUserResponse(user.Id, user.Status.ToString(), "User banned successfully"),
            "User banned"
        );
    }
}

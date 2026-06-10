using Mediator;
using Airbnb.UserService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Admin.SuspendUser;

public sealed class SuspendUserHandler(UserDbContext db)
    : ICommandHandler<Request, UserActionResponse>
{
    public async ValueTask<UserActionResponse> Handle(Request req, CancellationToken ct)
    {
        var user = await db.Users.FindAsync([req.Id], ct)
            ?? throw new NotFoundException("User not found.");

        user.Suspend(req.Reason);
        await db.SaveChangesAsync(ct);

        return new UserActionResponse(user.Id, user.Status.ToString(), "User suspended successfully");
    }
}

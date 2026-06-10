using Mediator;
using Airbnb.UserService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Admin.ActivateUser;

public sealed class ActivateUserHandler(UserDbContext db)
    : ICommandHandler<ActivateUserRequest, ActivateUserResponse>
{
    public async ValueTask<ActivateUserResponse> Handle(ActivateUserRequest req, CancellationToken ct)
    {
        var user = await db.Users.FindAsync([req.Id], ct)
            ?? throw new NotFoundException("User not found.");

        user.Activate();
        await db.SaveChangesAsync(ct);

        return new ActivateUserResponse(user.Id, user.Status.ToString(), "User activated successfully");
    }
}

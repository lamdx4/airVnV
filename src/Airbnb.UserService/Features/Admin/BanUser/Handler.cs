using Mediator;
using Airbnb.UserService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Admin.BanUser;

public sealed class BanUserHandler(UserDbContext db)
    : ICommandHandler<Request, BanUserResponse>
{
    public async ValueTask<BanUserResponse> Handle(Request req, CancellationToken ct)
    {
        var user = await db.Users.FindAsync([req.Id], ct)
            ?? throw new NotFoundException("User not found.");

        user.Ban(req.Reason);
        await db.SaveChangesAsync(ct);

        return new BanUserResponse(user.Id, user.Status.ToString(), "User banned successfully");
    }
}

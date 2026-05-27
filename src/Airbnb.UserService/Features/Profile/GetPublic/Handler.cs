using Airbnb.UserService.Infrastructure;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.UserService.Features.Profile.GetPublic;

public sealed class Handler(UserDbContext db) : IQueryHandler<Request, Response?>
{
    public async ValueTask<Response?> Handle(Request req, CancellationToken ct)
    {
        var profile = await db.UserProfiles
            .AsNoTracking()
            .Where(p => p.UserId == req.UserId)
            .Select(p => new Response(p.UserId, p.FullName, p.AvatarUrl))
            .FirstOrDefaultAsync(ct);

        return profile;
    }
}

using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.UserService.Infrastructure;

namespace Airbnb.UserService.Features.Profile.Get;

public record GetProfileRequest(Guid UserId) : Mediator.IQuery<Response?>;

public sealed class GetProfileHandler(UserDbContext db)
    : IQueryHandler<GetProfileRequest, Response?>
{
    public async ValueTask<Response?> Handle(GetProfileRequest req, CancellationToken ct)
    {
        return await db.Users
            .AsNoTracking()
            .Include(u => u.Profile)
            .Where(u => u.Id == req.UserId)
            .Select(u => new Response(
                u.Id,
                u.Email,
                u.Profile.FullName,
                u.Profile.AvatarUrl,
                u.Profile.PhoneNumber,
                u.Profile.Bio,
                u.Role
            ))
            .FirstOrDefaultAsync(ct);
    }
}

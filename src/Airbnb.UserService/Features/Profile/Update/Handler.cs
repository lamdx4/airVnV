using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Airbnb.UserService.Infrastructure;
using System.Text.Json.Serialization;

namespace Airbnb.UserService.Features.Profile.Update;

public class Handler(UserDbContext _db) : ICommandHandler<Request, Response>
{
    public async Task<Response> ExecuteAsync(Request cmd, CancellationToken ct)
    {
        var user = await _db.Users
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Id == cmd.UserId, ct);

        if (user == null) throw new InvalidOperationException("User not found");

        // Logic Domain nằm trong Entity (Rule 5)
        user.Profile.UpdateInfo(cmd.FullName, cmd.AvatarUrl, cmd.PhoneNumber, cmd.Bio);
        
        await _db.SaveChangesAsync(ct);

        return new Response(
            user.Id,
            user.Email,
            user.Profile.FullName,
            user.Profile.AvatarUrl,
            user.Profile.PhoneNumber,
            user.Profile.Bio,
            user.Role
        );
    }
}

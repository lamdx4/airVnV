using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Airbnb.UserService.Infrastructure;
using System.Text.Json.Serialization;

using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Profile.Update;

public class Handler(UserDbContext _db) : ICommandHandler<Request, ApiResponse<Response>>
{
    public async Task<ApiResponse<Response>> ExecuteAsync(Request cmd, CancellationToken ct)
    {
        var user = await _db.Users
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Id == cmd.UserId, ct);

        if (user == null) throw new InvalidOperationException("User not found");

        user.Profile.UpdateInfo(cmd.FullName, cmd.AvatarUrl, cmd.PhoneNumber, cmd.Bio);
        
        await _db.SaveChangesAsync(ct);

        return ApiResponse<Response>.SuccessResult(new Response(
            user.Id,
            user.Email,
            user.Profile.FullName,
            user.Profile.AvatarUrl,
            user.Profile.PhoneNumber,
            user.Profile.Bio,
            user.Role
        ), "Profile updated successfully");
    }
}

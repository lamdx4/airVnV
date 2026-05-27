using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.UserService.Infrastructure;
using Airbnb.UserService.Domain;

namespace Airbnb.UserService.Features.Admin.CreateAdmin;

public record Request(string Email, string Password, string FullName) : Mediator.ICommand<Response>;

public record Response(
    Guid UserId,
    string Email,
    string Role,
    string FullName
);

public class Handler(Airbnb.UserService.Infrastructure.UserDbContext _db) : ICommandHandler<Request, Response>
{
    public async ValueTask<Response> Handle(Request req, CancellationToken ct)
    {
        // Check if user exists
        if (await _db.Users.AnyAsync(u => u.Email == req.Email, ct))
        {
            throw new InvalidOperationException("Email already exists.");
        }

        // Create user with Admin role directly
        var user = new User(req.Email, req.Password, UserRole.Admin, req.FullName);

        _db.Users.Add(user);

        // Also add the profile
        var profile = new UserProfile(user.Id, req.FullName);
        _db.UserProfiles.Add(profile);

        await _db.SaveChangesAsync(ct);

        return new Response(user.Id, user.Email, user.Role.ToString(), req.FullName);
    }
}
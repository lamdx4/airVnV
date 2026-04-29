using FastEndpoints;
using FluentValidation;
using Airbnb.UserService.Domain;
using Airbnb.UserService.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.UserService.Features.RegisterUser;

public record Request(string FullName, string Email, string Password, UserRole Role);
public record Response(Guid Id, string FullName, string Email, UserRole Role);

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
        RuleFor(x => x.Role).IsInEnum();
    }
}

public class Endpoint : FastEndpoints.Endpoint<Request, Response>
{
    private readonly UserDbContext db;
    public Endpoint(UserDbContext db) => this.db = db;

    public override void Configure()
    {
        Post("/api/users/register");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var exists = await db.Users.AnyAsync(u => u.Email == req.Email, ct);
        if (exists)
        {
            await SendAsync(null!, 400, ct);
            return;
        }

        var user = new User(req.Email, req.Password, req.Role, req.FullName); 
        
        db.Users.Add(user);
        await db.SaveChangesAsync(ct);
        
        Response = new Response(user.Id, user.Profile.FullName, user.Email, user.Role);
    }
}

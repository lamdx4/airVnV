using FastEndpoints;
using FluentValidation;
using Airbnb.UserService.Domain;
using Airbnb.UserService.Infrastructure;

namespace Airbnb.UserService.Features.RegisterUser;

public record Request(string FullName, string Email, string Password);
public record Response(Guid Id, string FullName, string Email);

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
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
        var user = new User(req.FullName, req.Email, req.Password); 
        
        db.Users.Add(user);
        await db.SaveChangesAsync(ct);
        
        Response = new Response(user.Id, user.FullName, user.Email);
    }
}

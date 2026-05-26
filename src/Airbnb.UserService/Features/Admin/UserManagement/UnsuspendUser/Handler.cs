using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Admin.UserManagement.UnsuspendUser;

public record Request(Guid UserId) : Mediator.ICommand<Response>;

public record Response(
    Guid UserId,
    string Action,
    DateTimeOffset PerformedAt,
    string AdminId
);

public sealed class Handler(Airbnb.UserService.Infrastructure.UserDbContext db) : ICommandHandler<Request, Response>
{
    public async ValueTask<Response> Handle(Request req, CancellationToken ct)
    {
        var user = await db.Users
            .FirstOrDefaultAsync(u => u.Id == req.UserId, ct)
            ?? throw new NotFoundException("User not found.");

        // TODO: Get admin ID from JWT claims
        var adminId = "system";

        user.Unsuspend();
        await db.SaveChangesAsync(ct);

        return new Response(user.Id, "Unsuspended", DateTimeOffset.UtcNow, adminId);
    }
}

public sealed class Endpoint(IMediator mediator) : FastEndpoints.Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Post("/api/admin/users/{userId}/unsuspend");
        Policies("AdminOnly");
        Summary(s => {
            s.Summary = "Admin unsuspend a user account";
            s.Description = "Possible Error Codes: \n" +
                            "- **NOT_FOUND**: User not found.\n" +
                            "- **USER_NOT_SUSPENDED**: Only suspended users can be unsuspended.\n" +
                            "- **UNAUTHORIZED**: Not authorized to access this endpoint.";
            s.Responses[200] = "User unsuspended successfully.";
            s.Responses[404] = "User not found.";
            s.Responses[400] = "User is not suspended.";
            s.Responses[401] = "Unauthorized - Admin role required.";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await Send.ResponseAsync(ApiResponse<Response>.SuccessResult(result), cancellation: ct);
    }
}
using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Admin.UserManagement.SuspendUser;

public record Request(
    Guid UserId,
    string Reason,
    int? DurationDays = null
) : Mediator.ICommand<Response>;

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

        user.Suspend(req.Reason, req.DurationDays);
        await db.SaveChangesAsync(ct);

        return new Response(user.Id, "Suspended", DateTimeOffset.UtcNow, adminId);
    }
}

public sealed class Endpoint(IMediator mediator) : FastEndpoints.Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Post("/api/admin/users/{userId}/suspend");
        Policies("AdminOnly");
        Summary(s => {
            s.Summary = "Admin suspend a user account";
            s.Description = "Possible Error Codes: \n" +
                            "- **NOT_FOUND**: User not found.\n" +
                            "- **USER_BANNED**: Banned users cannot be suspended.\n" +
                            "- **UNAUTHORIZED**: Not authorized to access this endpoint.";
            s.Responses[200] = "User suspended successfully.";
            s.Responses[404] = "User not found.";
            s.Responses[400] = "User is banned and cannot be suspended.";
            s.Responses[401] = "Unauthorized - Admin role required.";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await Send.ResponseAsync(ApiResponse<Response>.SuccessResult(result), cancellation: ct);
    }
}
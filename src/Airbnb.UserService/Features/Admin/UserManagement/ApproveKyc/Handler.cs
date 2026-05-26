using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Admin.UserManagement.ApproveKyc;

public record Request(Guid UserId, string? Notes = null) : Mediator.ICommand<Response>;

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

        user.ApproveKyc();
        await db.SaveChangesAsync(ct);

        return new Response(user.Id, "KycApproved", DateTimeOffset.UtcNow, adminId);
    }
}

public sealed class Endpoint(IMediator mediator) : FastEndpoints.Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Post("/api/admin/users/{userId}/kyc/approve");
        Policies("AdminOnly");
        Summary(s => {
            s.Summary = "Admin approve user KYC (identity verification)";
            s.Description = "Possible Error Codes: \n" +
                            "- **NOT_FOUND**: User not found.\n" +
                            "- **KYC_NOT_PENDING**: Only pending KYC can be approved.\n" +
                            "- **UNAUTHORIZED**: Not authorized to access this endpoint.";
            s.Responses[200] = "KYC approved successfully.";
            s.Responses[404] = "User not found.";
            s.Responses[400] = "KYC is not in pending state.";
            s.Responses[401] = "Unauthorized - Admin role required.";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await Send.ResponseAsync(ApiResponse<Response>.SuccessResult(result), cancellation: ct);
    }
}
using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Admin.UserManagement.GetUserById;

public record Request(Guid UserId) : Mediator.IQuery<Response>;

public record Response(
    Guid Id,
    string Email,
    string FullName,
    string Role,
    string Status,
    string? AvatarUrl,
    bool IsVerified,
    string KycStatus,
    DateTime? KycSubmittedAt,
    DateTime? KycVerifiedAt,
    string? KycRejectionReason,
    string? PhoneNumber,
    string? Bio,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    int TotalBookings,
    int TotalProperties
);

public sealed class Handler(Airbnb.UserService.Infrastructure.UserDbContext db) : IQueryHandler<Request, Response>
{
    public async ValueTask<Response> Handle(Request req, CancellationToken ct)
    {
        var user = await db.Users
            .Include(u => u.Profile)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == req.UserId, ct)
            ?? throw new NotFoundException("User not found.");

        // TODO: Get actual stats from BookingService and PropertyService
        // For now, return placeholder values
        var totalBookings = 0;
        var totalProperties = 0;

        return new Response(
            user.Id,
            user.Email,
            user.Profile.FullName,
            user.Role.ToString(),
            user.Status.ToString(),
            user.Profile.AvatarUrl,
            user.KycStatus == Domain.KycStatus.Approved,
            user.KycStatus.ToString(),
            user.KycSubmittedAt,
            user.KycVerifiedAt,
            user.KycRejectionReason,
            user.Profile.PhoneNumber,
            user.Profile.Bio,
            user.CreatedAt,
            user.UpdatedAt,
            totalBookings,
            totalProperties
        );
    }
}

public sealed class Endpoint(IMediator mediator) : FastEndpoints.Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Get("/api/admin/users/{userId}");
        Policies("AdminOnly");
        Summary(s => {
            s.Summary = "Admin get user details by ID";
            s.Description = "Possible Error Codes: \n" +
                            "- **NOT_FOUND**: User not found.\n" +
                            "- **UNAUTHORIZED**: Not authorized to access this endpoint.";
            s.Responses[200] = "User details retrieved successfully.";
            s.Responses[404] = "User not found.";
            s.Responses[401] = "Unauthorized - Admin role required.";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await Send.ResponseAsync(ApiResponse<Response>.SuccessResult(result), cancellation: ct);
    }
}
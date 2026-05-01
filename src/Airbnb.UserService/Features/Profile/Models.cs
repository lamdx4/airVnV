using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Airbnb.UserService.Infrastructure;
using System.Security.Claims;
using Airbnb.UserService.Domain;

namespace Airbnb.UserService.Features.Profile;

public record ProfileResponse(
    Guid UserId,
    string Email,
    string FullName,
    string? AvatarUrl,
    string? PhoneNumber,
    string? Bio,
    UserRole Role
);

// --- [QUERY] GET PROFILE ---
// Query đơn giản có thể truy cập DbContext trực tiếp theo quy tắc dự án
public class GetEndpoint(UserDbContext _db) : EndpointWithoutRequest<ProfileResponse>
{
    public override void Configure()
    {
        Get("/api/users/me");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userIdClaim = User.FindFirstValue("UserId");
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            await SendAsync(null!, 401, ct);
            return;
        }

        var user = await _db.Users
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Id == userId, ct);

        if (user == null)
        {
            await SendAsync(null!, 404, ct);
            return;
        }

        Response = new ProfileResponse(
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

// --- [COMMAND] UPDATE PROFILE ---
public record UpdateRequest(
    string FullName,
    string? AvatarUrl,
    string? PhoneNumber,
    string? Bio
);

public class UpdateValidator : Validator<UpdateRequest>
{
    public UpdateValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(255);
        RuleFor(x => x.PhoneNumber).MaximumLength(20);
        RuleFor(x => x.Bio).MaximumLength(500);
    }
}

// Application Layer: Handler xử lý logic nghiệp vụ
public class UpdateHandler(UserDbContext _db)
{
    public async Task<ProfileResponse?> HandleAsync(Guid userId, UpdateRequest req, CancellationToken ct)
    {
        var user = await _db.Users
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Id == userId, ct);

        if (user == null) return null;

        // Domain Logic
        user.Profile.UpdateInfo(req.FullName, req.AvatarUrl, req.PhoneNumber, req.Bio);
        
        await _db.SaveChangesAsync(ct);

        return new ProfileResponse(
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

// Web Layer: Endpoint duyệt HTTP
public class UpdateEndpoint(UpdateHandler _handler) : Endpoint<UpdateRequest, ProfileResponse>
{
    public override void Configure()
    {
        Put("/api/users/me");
    }

    public override async Task HandleAsync(UpdateRequest req, CancellationToken ct)
    {
        var userIdClaim = User.FindFirstValue("UserId");
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            await SendAsync(null!, 401, ct);
            return;
        }

        var result = await _handler.HandleAsync(userId, req, ct);

        if (result == null)
        {
            await SendAsync(null!, 404, ct);
            return;
        }

        Response = result;
    }
}

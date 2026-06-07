using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Airbnb.ServiceDefaults.Infrastructure;
using Airbnb.UserService.Infrastructure;

namespace Airbnb.UserService.Features.Internal.GetUserBasicInfos;

public record Request
{
    public List<Guid> Ids { get; init; } = new();
}

public record UserBasicInfo(
    Guid Id,
    string FullName,
    string Email,
    string? AvatarUrl
);

public record Response(List<UserBasicInfo> Items);

/// <summary>
/// Internal service-to-service endpoint. Returns a batch of users' basic info
/// (name, email, avatar) by Id. Used by PaymentService to enrich host data.
/// </summary>
public class Endpoint(UserDbContext db) : Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Post("/api/internal/users/basic-infos");
        AllowAnonymous();
        Summary(s => s.Summary = "Internal: batch lookup user basic info by Ids");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        if (req.Ids.Count == 0)
        {
            await Send.ResponseAsync(ApiResponse<Response>.SuccessResult(new Response(new())), cancellation: ct);
            return;
        }

        var ids = req.Ids.Distinct().Take(200).ToList(); // cap at 200 per call

        var users = await db.Users.AsNoTracking()
            .Where(u => ids.Contains(u.Id))
            .Select(u => new UserBasicInfo(
                u.Id,
                u.Profile.FullName,
                u.Email,
                u.Profile.AvatarUrl
            ))
            .ToListAsync(ct);

        await Send.ResponseAsync(
            ApiResponse<Response>.SuccessResult(new Response(users)),
            cancellation: ct);
    }
}

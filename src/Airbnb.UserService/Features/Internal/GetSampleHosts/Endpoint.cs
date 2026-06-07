using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Airbnb.ServiceDefaults.Infrastructure;
using Airbnb.UserService.Domain;
using Airbnb.UserService.Infrastructure;

namespace Airbnb.UserService.Features.Internal.GetSampleHosts;

public record Request
{
    public int Count { get; init; } = 3;
}

public record HostStub(Guid Id, string FullName);

public record Response(List<HostStub> Items);

/// <summary>
/// Internal helper for demo/seed scripts in other services (e.g. PaymentService
/// bootstrap). Returns the first N regular users to use as fake hosts.
/// </summary>
public class Endpoint(UserDbContext db) : Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Get("/api/internal/users/sample-hosts");
        AllowAnonymous();
        Summary(s => s.Summary = "Internal: get N sample users to use as hosts in dev seeds");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var count = req.Count is < 1 or > 50 ? 3 : req.Count;

        var users = await db.Users.AsNoTracking()
            .Where(u => u.Role == UserRole.User && u.Status == UserStatus.Active)
            .OrderBy(u => u.CreatedAt)
            .Take(count)
            .Select(u => new HostStub(u.Id, u.Profile.FullName))
            .ToListAsync(ct);

        await Send.ResponseAsync(
            ApiResponse<Response>.SuccessResult(new Response(users)),
            cancellation: ct);
    }
}

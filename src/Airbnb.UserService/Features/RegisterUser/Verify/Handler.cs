using FastEndpoints;
using Microsoft.Extensions.Caching.Memory;
using Airbnb.UserService.Infrastructure;
using Airbnb.UserService.Domain;

namespace Airbnb.UserService.Features.RegisterUser.Verify;

public class Handler(UserDbContext _db, IMemoryCache _cache) : ICommandHandler<Request, Response>
{
    public async Task<Response> ExecuteAsync(Request req, CancellationToken ct)
    {
        if (!_cache.TryGetValue($"verify_{req.Email}", out (User user, string code) cached))
        {
            throw new InvalidOperationException("Verification code expired or not found.");
        }

        if (cached.code != req.Code)
        {
            throw new InvalidOperationException("Invalid verification code.");
        }

        // Lưu vào Database thực tế
        _db.Users.Add(cached.user);
        await _db.SaveChangesAsync(ct);
        
        // Dọn cache
        _cache.Remove($"verify_{req.Email}");

        return new Response("Email verified and account created successfully.");
    }
}

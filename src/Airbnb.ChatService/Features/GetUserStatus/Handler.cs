using Mediator;
using Microsoft.Extensions.Caching.Distributed;

namespace Airbnb.ChatService.Features.GetUserStatus;

public sealed class GetUserStatusHandler(IDistributedCache cache) 
    : IQueryHandler<Request, Response>
{
    public async ValueTask<Response> Handle(
        Request req, CancellationToken ct)
    {
        var status = await cache.GetStringAsync($"presence:user:{req.UserId}", ct);
        var isOnline = status == "online";
        return new Response(isOnline);
    }
}

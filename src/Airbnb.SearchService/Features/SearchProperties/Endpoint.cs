using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;
using Airbnb.SearchService.Domain;

namespace Airbnb.SearchService.Features.SearchProperties;

public class Endpoint(IMediator mediator) : Endpoint<Request, ApiResponse<PagedResponse<PropertyDoc>>>
{
    public override void Configure()
    {
        Get("/api/search");
        AllowAnonymous();
        Options(x => x.CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)))); // Dùng Output Caching (Redis) thay vì ResponseCache cổ điển
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await Send.ResponseAsync(ApiResponse<PagedResponse<PropertyDoc>>.SuccessResult(result), cancellation: ct);
    }
}

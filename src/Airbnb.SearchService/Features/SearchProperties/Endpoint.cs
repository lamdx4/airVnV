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
        ResponseCache(60); // Cache 60 giây (Tích hợp Output Caching)
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await Send.ResponseAsync(ApiResponse<PagedResponse<PropertyDoc>>.SuccessResult(result), cancellation: ct);
    }
}

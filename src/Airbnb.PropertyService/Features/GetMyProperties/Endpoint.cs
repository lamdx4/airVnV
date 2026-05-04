using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.GetMyProperties;

public class Endpoint(IMediator mediator)
    : FastEndpoints.Endpoint<Request, ApiResponse<PagedResponse<PropertyResponse>>>
{
    public override void Configure()
    {
        Get("/api/properties/my");
        Summary(s => {
            s.Summary = "Get all properties of the current host with pagination";
            s.Description = "Returns a paged list of properties owned by the authenticated host.";
            s.Responses[200] = "List of properties retrieved successfully.";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        // Trích xuất UserId từ Header (được Gateway gắn vào)
        if (!HeaderExists("X-User-Id") || !Guid.TryParse(Header("X-User-Id"), out var userId))
        {
            throw new UnauthorizedAccessException("User identification missing.");
        }

        var internalReq = new InternalRequest(userId, req.PageNumber, req.PageSize, req.SearchTerm, req.Status);
        
        var result = await mediator.Send(internalReq, ct);
        await SendAsync(ApiResponse<PagedResponse<PropertyResponse>>.SuccessResult(result), cancellation: ct);
    }
}

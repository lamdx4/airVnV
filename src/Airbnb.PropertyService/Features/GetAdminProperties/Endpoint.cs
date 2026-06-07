using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.GetAdminProperties;

public class Endpoint(IMediator mediator)
    : FastEndpoints.Endpoint<Request, ApiResponse<PagedResponse<AdminPropertyResponse>>>
{
    public override void Configure()
    {
        Get("/api/properties/admin");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Admin: list all properties with pagination and filters";
            s.Description = "Returns a paged list of all properties for admin moderation. " +
                            "Supports filtering by status and searching by title. " +
                            "Requires Admin role (X-User-Role header from Gateway).";
            s.Responses[200] = "Paginated list of properties.";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        // TODO: Thêm Admin role check khi Gateway forward role header
        var result = await mediator.Send(req, ct);
        await Send.ResponseAsync(ApiResponse<PagedResponse<AdminPropertyResponse>>.SuccessResult(result), cancellation: ct);
    }
}

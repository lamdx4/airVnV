using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.Admin.GetPropertyDetail;

public class Endpoint(IMediator mediator)
    : FastEndpoints.Endpoint<Request, ApiResponse<AdminPropertyDetailDto>>
{
    public override void Configure()
    {
        Get("/api/admin/properties/{PropertyId}");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Admin: get property detail by ID";
            s.Description = "Returns full property details including images, amenities, and moderation fields for admin review.";
            s.Responses[200] = "Property detail retrieved successfully.";
            s.Responses[404] = "Property not found.";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await Send.ResponseAsync(ApiResponse<AdminPropertyDetailDto>.SuccessResult(result), cancellation: ct);
    }
}

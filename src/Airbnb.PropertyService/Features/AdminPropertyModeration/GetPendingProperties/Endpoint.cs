using Airbnb.ServiceDefaults.Infrastructure;
using FastEndpoints;
using Mediator;

namespace Airbnb.PropertyService.Features.AdminPropertyModeration.GetPendingProperties;

public class Endpoint(IMediator mediator) : FastEndpoints.Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Get("/api/admin/properties/pending");
        Summary(s => {
            s.Summary = "Admin get pending properties for moderation queue";
            s.Description = "Possible Error Codes: \n" +
                            "- **PROPERTY_NOT_FOUND**: Property not found.";
            s.Responses[200] = "List of pending properties retrieved successfully.";
            s.Responses[400] = "Invalid request parameters.";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await Send.ResponseAsync(ApiResponse<Response>.SuccessResult(result), cancellation: ct);
    }
}
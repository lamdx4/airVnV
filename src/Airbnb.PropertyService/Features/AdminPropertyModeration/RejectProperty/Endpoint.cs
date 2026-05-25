using Airbnb.ServiceDefaults.Infrastructure;
using FastEndpoints;
using Mediator;

namespace Airbnb.PropertyService.Features.AdminPropertyModeration.RejectProperty;

public class Endpoint(IMediator mediator) : FastEndpoints.Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Post("/api/admin/properties/{PropertyId}/reject");
        Summary(s => {
            s.Summary = "Admin reject property (PendingReview → Archived)";
            s.Description = "Possible Error Codes: \n" +
                            "- **PROPERTY_NOT_FOUND**: Property not found.\n" +
                            "- **PROPERTY_NOT_IN_REVIEW**: Only properties pending review can be rejected.";
            s.Responses[200] = "Property rejected and archived.";
            s.Responses[400] = "Property not in correct state.";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await Send.ResponseAsync(ApiResponse<Response>.SuccessResult(result), cancellation: ct);
    }
}
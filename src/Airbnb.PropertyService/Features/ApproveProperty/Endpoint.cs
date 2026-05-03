using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.ApproveProperty;

public class Endpoint(IMediator mediator)
    : FastEndpoints.Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Post("/api/properties/{PropertyId}/approve");
        Summary(s => {
            s.Summary = "Admin approve property (PendingReview → Published)";
            s.Description = "Possible Error Codes: \n" +
                            "- **PROPERTY_NOT_FOUND**: Property not found.\n" +
                            "- **PROPERTY_NOT_IN_REVIEW**: Only properties pending review can be approved.";
            s.Responses[200] = "Property approved and published.";
            s.Responses[400] = "Property not in correct state.";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await SendAsync(ApiResponse<Response>.SuccessResult(result), cancellation: ct);
    }
}

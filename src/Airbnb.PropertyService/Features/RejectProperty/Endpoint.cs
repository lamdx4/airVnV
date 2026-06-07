using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.RejectProperty;

public class Endpoint(IMediator mediator)
    : FastEndpoints.Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Post("/api/properties/{PropertyId}/reject");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Admin reject property (PendingReview → Rejected)";
            s.Description = "Possible Error Codes: \n" +
                            "- **PROPERTY_NOT_FOUND**: Property not found.\n" +
                            "- **PROPERTY_NOT_IN_REVIEW**: Only properties pending review can be rejected.\n" +
                            "- **PROPERTY_REJECTION_REASON_REQUIRED**: Reason is mandatory.\n" +
                            "- **PROPERTY_REJECTION_REASON_TOO_SHORT**: Reason must be at least 10 characters.";
            s.Responses[200] = "Property rejected.";
            s.Responses[400] = "Property not in correct state or validation error.";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await Send.ResponseAsync(ApiResponse<Response>.SuccessResult(result), cancellation: ct);
    }
}

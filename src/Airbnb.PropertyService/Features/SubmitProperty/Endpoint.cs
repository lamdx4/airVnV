using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.SubmitProperty;

public class Endpoint(IMediator mediator)
    : FastEndpoints.Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Post("/api/properties/{PropertyId}/submit");
        Summary(s => {
            s.Summary = "Host submits property for review (Draft → PendingReview)";
            s.Description = "Possible Error Codes: \n" +
                            "- **PROPERTY_NOT_FOUND**: Property not found or access denied.\n" +
                            "- **PROPERTY_INVALID_STATUS**: Only Draft properties can be submitted.\n" +
                            "- **PROPERTY_COVER_IMAGE_REQUIRED**: A cover image is required before submission.";
            s.Responses[200] = "Property submitted successfully.";
            s.Responses[400] = "Business rule violation.";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await SendAsync(ApiResponse<Response>.SuccessResult(result), cancellation: ct);
    }
}

using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.UpdateStatus;

public class Endpoint(IMediator mediator)
    : FastEndpoints.Endpoint<Request, ApiResponse<bool>>
{
    public override void Configure()
    {
        Patch("/api/properties/{PropertyId}/status");
        Summary(s => {
            s.Summary = "Update property status (Publish/Unpublish/Archive)";
            s.Description = 
                """
                Changes the lifecycle status of a property.
                
                **Error Codes:**
                - **PROPERTY_NOT_FOUND**: Property not found or unauthorized access.
                - **PROPERTY_INVALID_STATUS**: Current status does not allow transition to target status.
                - **PROPERTY_TITLE_TOO_SHORT**: Title must be at least 10 characters.
                - **PROPERTY_DESCRIPTION_TOO_SHORT**: Description must be at least 20 characters.
                - **PROPERTY_PRICE_REQUIRED**: Base price must be set before publishing.
                - **PROPERTY_MIN_IMAGES_REQUIRED**: At least 5 images are required to publish.
                - **PROPERTY_COVER_IMAGE_REQUIRED**: A cover image is required for publishing.
                """;
            s.Responses[400] = "Validation error or business rule violation.";
            s.Responses[404] = "Property not found.";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await SendAsync(result, cancellation: ct);
    }
}

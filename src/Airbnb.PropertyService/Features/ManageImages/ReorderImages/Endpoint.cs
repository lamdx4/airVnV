using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.ManageImages.ReorderImages;

public class Endpoint(IMediator mediator)
    : FastEndpoints.Endpoint<Request, ApiResponse<bool>>
{
    public override void Configure()
    {
        Verbs(Http.PUT, Http.POST);
        Routes("/api/properties/{PropertyId}/images/reorder");
        AllowAnonymous();
        Summary(s => {
            s.Summary = "Reorder property images";
            s.Description = 
                """
                Updates the display order of property images.
                
                **Error Codes:**
                - **PROPERTY_NOT_FOUND**: Property not found or unauthorized access.
                - **PROPERTY_IMAGE_NOT_FOUND**: One or more image IDs were not found in this property.
                """;
            s.Responses[400] = "Validation error or business rule violation.";
            s.Responses[404] = "Property not found.";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await Send.ResponseAsync(result, cancellation: ct);
    }
}

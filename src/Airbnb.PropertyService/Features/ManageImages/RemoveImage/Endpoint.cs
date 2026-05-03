using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.ManageImages.RemoveImage;

public class Endpoint(IMediator mediator)
    : FastEndpoints.Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Delete("/api/properties/{PropertyId}/images/{ImageId}");
        Summary(s => {
            s.Summary = "Remove property image (Host only)";
            s.Description = "Possible Error Codes: \n" +
                            "- **PROPERTY_NOT_FOUND**: Property not found or access denied.\n" +
                            "- **PROPERTY_IMAGE_NOT_FOUND**: Image does not exist.\n" +
                            "- **PROPERTY_CANNOT_REMOVE_LAST_COVER**: Cannot remove the only cover of a published property.";
            s.Responses[200] = "Image removed physically and from DB.";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await SendAsync(ApiResponse<Response>.SuccessResult(result), cancellation: ct);
    }
}

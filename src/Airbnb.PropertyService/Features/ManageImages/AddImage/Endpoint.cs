using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.ManageImages.AddImage;

public class Endpoint(IMediator mediator)
    : FastEndpoints.Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Post("/api/properties/{PropertyId}/images");
        AllowFileUploads();
        Summary(s => {
            s.Summary = "Add image to property (Host only, Server-side upload)";
            s.Description = "Possible Error Codes: \n" +
                            "- **PROPERTY_NOT_FOUND**: Property not found or access denied.\n" +
                            "- **PROPERTY_COVER_IMAGE_EXISTS**: Only one cover image is allowed.";
            s.Responses[200] = "Image uploaded and saved successfully.";
            s.Responses[400] = "Invalid file or business rule violation.";
            s.Responses[404] = "Property not found.";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await SendAsync(ApiResponse<Response>.SuccessResult(result), cancellation: ct);
    }
}

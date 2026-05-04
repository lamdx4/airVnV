using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.ManageImages.AddImages;

public class Endpoint(IMediator mediator)
    : FastEndpoints.Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Post("/api/properties/{PropertyId}/images/bulk");
        AllowFileUploads();
        Summary(s => {
            s.Summary = "Bulk add images to property (Host only)";
            s.Description = "Upload multiple images at once. Recommended for gallery creation.";
            s.Responses[200] = "All images uploaded successfully.";
            s.Responses[400] = "One or more files invalid.";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await SendAsync(ApiResponse<Response>.SuccessResult(result), cancellation: ct);
    }
}

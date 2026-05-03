using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.UpdateProperty;

public class Endpoint(IMediator mediator)
    : FastEndpoints.Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Patch("/api/properties/{PropertyId}");
        Summary(s => {
            s.Summary = "Update property information (Host only, partial update)";
            s.Description = "Possible Error Codes: \n" +
                            "- **PROPERTY_NOT_FOUND**: Property not found or access denied.\n" +
                            "- **PROPERTY_TITLE_REQUIRED**: Title cannot be empty.";
            s.Responses[200] = "Property updated successfully.";
            s.Responses[400] = "Invalid input or business rule violation.";
            s.Responses[404] = "Property not found.";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await SendAsync(ApiResponse<Response>.SuccessResult(result), cancellation: ct);
    }
}

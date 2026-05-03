using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.DeleteProperty;

public class Endpoint(IMediator mediator)
    : FastEndpoints.Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Delete("/api/properties/{PropertyId}");
        Summary(s => {
            s.Summary = "Delete property (Host only, Draft or Archived only)";
            s.Description = "Possible Error Codes: \n" +
                            "- **PROPERTY_NOT_FOUND**: Property not found or access denied.\n" +
                            "- **PROPERTY_CANNOT_BE_DELETED**: Only Draft or Archived properties can be deleted.";
            s.Responses[200] = "Property deleted successfully.";
            s.Responses[400] = "Property is in a state that cannot be deleted.";
            s.Responses[404] = "Property not found.";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await SendAsync(ApiResponse<Response>.SuccessResult(result), cancellation: ct);
    }
}

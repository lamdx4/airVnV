using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.AdminDeleteProperty;

public class Endpoint(IMediator mediator)
    : FastEndpoints.Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Delete("/api/properties/{PropertyId}/admin-delete");
        Summary(s =>
        {
            s.Summary = "Admin: emergency delete property (any status)";
            s.Description = "Admin-only endpoint to remove a property regardless of its current status. " +
                            "Use for emergency removal of listings that pose safety/legal risk. " +
                            "Performs hard delete from database.\n\n" +
                            "Possible Error Codes: \n" +
                            "- **PROPERTY_NOT_FOUND**: Property not found.";
            s.Responses[200] = "Property deleted successfully.";
            s.Responses[404] = "Property not found.";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await Send.ResponseAsync(ApiResponse<Response>.SuccessResult(result), cancellation: ct);
    }
}

using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.ManageAvailability.RemoveAvailability;

public class Endpoint(IMediator mediator)
    : FastEndpoints.Endpoint<Request, ApiResponse<bool>>
{
    public override void Configure()
    {
        Delete("/api/properties/{PropertyId}/availability/{AvailabilityId}");
        Summary(s => {
            s.Summary = "Remove property availability/blocked dates";
            s.Description = 
                """
                Deletes a blocked date range record, making those dates available again.
                
                **Error Codes:**
                - **PROPERTY_NOT_FOUND**: Property not found or unauthorized access.
                - **AVAILABILITY_NOT_FOUND**: The availability record was not found.
                """;
            s.Responses[400] = "Validation error or business rule violation.";
            s.Responses[404] = "Property or Availability record not found.";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await Send.ResponseAsync(result, cancellation: ct);
    }
}

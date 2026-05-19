using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.UpdateLocation;

public class Endpoint(IMediator mediator)
    : FastEndpoints.Endpoint<Request, ApiResponse<bool>>
{
    public override void Configure()
    {
        Put("/api/properties/{PropertyId}/location");
        Summary(s => {
            s.Summary = "Update property location coordinates and address";
            s.Description = 
                """
                Updates Latitude, Longitude, and detailed AddressRaw for a listing.
                
                **Error Codes:**
                - **PROPERTY_NOT_FOUND**: Property not found or unauthorized access.
                """;
            s.Responses[400] = "Validation error.";
            s.Responses[404] = "Property not found.";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await Send.ResponseAsync(result, cancellation: ct);
    }
}

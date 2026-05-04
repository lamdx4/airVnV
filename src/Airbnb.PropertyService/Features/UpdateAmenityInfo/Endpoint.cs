using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.UpdateAmenityInfo;

public class Endpoint(IMediator mediator)
    : FastEndpoints.Endpoint<Request, ApiResponse<bool>>
{
    public override void Configure()
    {
        Patch("/api/properties/{PropertyId}/amenities/{AmenityId}");
        Summary(s => {
            s.Summary = "Update amenity additional information/notes";
            s.Description = 
                """
                Allows the host to add notes like Wifi Password or instructions for a specific amenity.
                
                **Error Codes:**
                - **PROPERTY_NOT_FOUND**: Property not found or unauthorized access.
                - **PROPERTY_AMENITY_NOT_FOUND**: The specified amenity is not associated with this property.
                """;
            s.Responses[400] = "Validation error or business rule violation.";
            s.Responses[404] = "Property or Amenity not found.";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await SendAsync(result, cancellation: ct);
    }
}

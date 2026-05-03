using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.ManageAmenities.RemoveAmenity;

public class Endpoint(IMediator mediator)
    : FastEndpoints.Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Delete("/api/properties/{PropertyId}/amenities/{AmenityId}");
        Summary(s => {
            s.Summary = "Remove amenity from property (Host only)";
            s.Description = "Possible Error Codes: \n" +
                            "- **PROPERTY_NOT_FOUND**: Property not found or access denied.\n" +
                            "- **PROPERTY_AMENITY_NOT_FOUND**: Amenity not linked to this property.";
            s.Responses[200] = "Amenity unlinked successfully.";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await SendAsync(ApiResponse<Response>.SuccessResult(result), cancellation: ct);
    }
}

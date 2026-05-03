using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.ManageAmenities.AddAmenity;

public class Endpoint(IMediator mediator)
    : FastEndpoints.Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Post("/api/properties/{PropertyId}/amenities/{AmenityId}");
        Summary(s => {
            s.Summary = "Add amenity to property (Host only)";
            s.Description = "Possible Error Codes: \n" +
                            "- **PROPERTY_NOT_FOUND**: Property not found or access denied.\n" +
                            "- **PROPERTY_AMENITY_NOT_FOUND**: Amenity ID is not in global catalog.\n" +
                            "- **PROPERTY_AMENITY_EXISTS**: Amenity already added to this property.";
            s.Responses[200] = "Amenity linked successfully.";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await SendAsync(ApiResponse<Response>.SuccessResult(result), cancellation: ct);
    }
}

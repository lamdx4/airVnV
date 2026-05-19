using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.GetAvailableAmenities;

public class Endpoint(IMediator mediator)
    : FastEndpoints.EndpointWithoutRequest<ApiResponse<List<AmenityResponse>>>
{
    public override void Configure()
    {
        Get("/api/amenities");
        AllowAnonymous(); // Ai cũng có thể xem danh sách tiện nghi
        Summary(s => {
            s.Summary = "Get all available amenities in the system";
            s.Responses[200] = "List of amenities retrieved successfully.";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await mediator.Send(new Request(), ct);
        await Send.ResponseAsync(result, cancellation: ct);
    }
}

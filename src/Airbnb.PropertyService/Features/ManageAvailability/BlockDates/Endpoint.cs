using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.ManageAvailability.BlockDates;

public class Endpoint(IMediator mediator)
    : FastEndpoints.Endpoint<Request, ApiResponse<bool>>
{
    public override void Configure()
    {
        Post("/api/properties/{PropertyId}/availability/block");
        Summary(s => {
            s.Summary = "Block property dates (Calendar Management)";
            s.Description = 
                """
                Allows hosts to mark specific dates as unavailable for booking.
                
                **Error Codes:**
                - **PROPERTY_NOT_FOUND**: Property not found or unauthorized access.
                - **AVAILABILITY_INVALID_RANGE**: Start date must be before end date.
                - **AVAILABILITY_PAST_DATE**: Cannot block dates in the past.
                - **AVAILABILITY_OVERLAP**: Selected dates overlap with existing blocked dates.
                """;
            s.Responses[400] = "Validation error or business rule violation.";
            s.Responses[404] = "Property not found.";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await SendAsync(result, cancellation: ct);
    }
}

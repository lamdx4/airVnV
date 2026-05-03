using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.SuspendProperty;

public class Endpoint(IMediator mediator)
    : FastEndpoints.Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Post("/api/properties/{PropertyId}/suspend");
        Summary(s => {
            s.Summary = "Admin suspends property (Published → Suspended)";
            s.Description = "Possible Error Codes: \n" +
                            "- **PROPERTY_NOT_FOUND**: Property not found.\n" +
                            "- **PROPERTY_NOT_PUBLISHED**: Only published properties can be suspended.\n" +
                            "- **PROPERTY_SUSPENSION_REASON_REQUIRED**: Reason is mandatory.";
            s.Responses[200] = "Property suspended.";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await SendAsync(ApiResponse<Response>.SuccessResult(result), cancellation: ct);
    }
}

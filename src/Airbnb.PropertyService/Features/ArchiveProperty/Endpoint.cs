using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.ArchiveProperty;

public class Endpoint(IMediator mediator)
    : FastEndpoints.Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Post("/api/properties/{PropertyId}/archive");
        Summary(s => {
            s.Summary = "Host archives property (Published|Suspended → Archived)";
            s.Description = "Possible Error Codes: \n" +
                            "- **PROPERTY_NOT_FOUND**: Property not found or access denied.\n" +
                            "- **PROPERTY_INVALID_ARCHIVE_STATUS**: Only Published or Suspended properties can be archived.";
            s.Responses[200] = "Property archived.";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await SendAsync(ApiResponse<Response>.SuccessResult(result), cancellation: ct);
    }
}

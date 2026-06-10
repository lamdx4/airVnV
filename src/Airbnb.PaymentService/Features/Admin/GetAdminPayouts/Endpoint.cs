using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Features.Admin.GetAdminPayouts;

public class Endpoint(IMediator mediator) : Endpoint<Request, ApiResponse<PagedResponse<AdminPayoutItem>>>
{
    public override void Configure()
    {
        Get("/api/admin/payouts");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Admin: list payouts owed to hosts";
            s.Description = "**Possible error codes:**\n- `VALIDATION_ERROR` — invalid query parameters";
            s.Responses[200] = "Payouts retrieved";
            s.Responses[400] = "Validation error";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        Response = ApiResponse<PagedResponse<AdminPayoutItem>>.SuccessResult(result);
    }
}

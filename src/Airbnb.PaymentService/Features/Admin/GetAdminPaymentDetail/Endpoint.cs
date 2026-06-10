using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Features.Admin.GetAdminPaymentDetail;

public class Endpoint(IMediator mediator) : Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Get("/api/admin/payments/{id}");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Admin: get a single pay-in transaction";
            s.Description = "**Possible error codes:**\n- `NOT_FOUND` — payment does not exist";
            s.Responses[200] = "Payment detail retrieved";
            s.Responses[404] = "Payment not found";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        Response = ApiResponse<Response>.SuccessResult(result);
    }
}

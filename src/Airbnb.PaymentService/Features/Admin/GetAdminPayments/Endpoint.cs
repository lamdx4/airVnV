using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Features.Admin.GetAdminPayments;

public class Endpoint(IMediator mediator) : Endpoint<Request, ApiResponse<PagedResponse<AdminPaymentItem>>>
{
    public override void Configure()
    {
        Get("/api/admin/payments");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Admin: list pay-in transactions with filters & pagination";
            s.Description = "**Possible error codes:**\n- `VALIDATION_ERROR` — invalid query parameters";
            s.Responses[200] = "Payments retrieved successfully";
            s.Responses[400] = "Validation error";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        Response = ApiResponse<PagedResponse<AdminPaymentItem>>.SuccessResult(result);
    }
}

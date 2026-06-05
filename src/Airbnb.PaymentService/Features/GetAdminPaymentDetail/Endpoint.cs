using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Features.GetAdminPaymentDetail;

public class Endpoint(IMediator mediator)
    : FastEndpoints.Endpoint<Request, ApiResponse<AdminPaymentDetailResponse>>
{
    public override void Configure()
    {
        Get("/api/payments/admin/{PaymentId}");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Admin: get payment detail with refund history";
            s.Description = "Returns full payment details including refund records for admin financial review.";
            s.Responses[200] = "Payment detail with refunds.";
            s.Responses[404] = "Payment not found.";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await Send.ResponseAsync(ApiResponse<AdminPaymentDetailResponse>.SuccessResult(result), cancellation: ct);
    }
}

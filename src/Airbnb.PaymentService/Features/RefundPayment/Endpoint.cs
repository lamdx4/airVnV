using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Features.RefundPayment;

public class Request
{
    public Guid PaymentId { get; set; }
    public decimal? Amount { get; set; }
    public string Reason { get; set; } = default!;
    public Guid? TicketId { get; set; }
}

public class Endpoint(IMediator mediator)
    : FastEndpoints.Endpoint<Request, ApiResponse<RefundPaymentResponse>>
{
    public override void Configure()
    {
        Post("/api/payments/admin/{PaymentId}/refund");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Admin: refund a payment (full or partial)";
            s.Description = "Issues a refund for a successful or partially refunded payment. " +
                            "If Amount is omitted or >= remaining amount, a full refund is issued. " +
                            "Reason is mandatory.";
            s.Responses[200] = "Refund processed.";
            s.Responses[400] = "Invalid refund request (wrong status, amount exceeded, etc.).";
            s.Responses[404] = "Payment not found.";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var command = new Command(req.PaymentId, req.Amount, req.Reason, req.TicketId);
        var result = await mediator.Send(command, ct);
        await Send.ResponseAsync(ApiResponse<RefundPaymentResponse>.SuccessResult(result), cancellation: ct);
    }
}

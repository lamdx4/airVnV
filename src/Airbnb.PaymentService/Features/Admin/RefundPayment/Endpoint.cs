using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;
using System.Security.Claims;

namespace Airbnb.PaymentService.Features.Admin.RefundPayment;

public record Request
{
    public Guid Id { get; init; }              // PaymentId (route param)
    public decimal Amount { get; init; }       // amount to refund
    public string Reason { get; init; } = string.Empty;
    public Guid? TicketId { get; init; }
}

public record Response(
    Guid RefundId,
    Guid PaymentId,
    decimal RefundedNow,
    decimal TotalRefunded,
    bool IsFullRefund);

public class Endpoint(IMediator mediator) : Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Post("/api/admin/payments/{id}/refund");
        AllowAnonymous();
        Summary(s => s.Summary = "Admin: refund a payment (full or partial).");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var adminId);

        var result = await mediator.Send(
            new Features.RefundPayment.Command(
                req.Id,
                req.Amount,
                req.Reason,
                adminId == Guid.Empty ? Guid.CreateVersion7() : adminId,
                req.TicketId),
            ct);

        var response = new Response(
            result.RefundId, result.PaymentId, result.RefundedNow, result.TotalRefunded, result.IsFullRefund);
        await Send.ResponseAsync(ApiResponse<Response>.SuccessResult(response, "Refund issued"), cancellation: ct);
    }
}

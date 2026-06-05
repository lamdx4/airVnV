using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Features.GetAdminPayments;

public class Endpoint(IMediator mediator)
    : FastEndpoints.Endpoint<Request, ApiResponse<PagedResponse<AdminPaymentResponse>>>
{
    public override void Configure()
    {
        Get("/api/payments/admin");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Admin: list all payments with pagination and filters";
            s.Description = "Returns a paged list of all payments for admin financial oversight. " +
                            "Supports filtering by status, date range, and searching by transaction ID. " +
                            "Requires Admin role (X-User-Role header from Gateway).";
            s.Responses[200] = "Paginated list of payments.";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await Send.ResponseAsync(ApiResponse<PagedResponse<AdminPaymentResponse>>.SuccessResult(result), cancellation: ct);
    }
}

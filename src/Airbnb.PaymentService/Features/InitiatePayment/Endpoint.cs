using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;
using System.Security.Claims;

namespace Airbnb.PaymentService.Features.InitiatePayment;

public class Endpoint(IMediator mediator)
    : FastEndpoints.Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Post("/api/payments/initiate");
        AllowAnonymous();
        Summary(s =>
        {
            s.Description = "Initiates a payment for a booking. Automatically resolves the correct provider based on country.";
            s.Responses[400] = "Booking not found or invalid request.";
            s.Responses[403] = "Unauthorized access to booking.";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        if (req.UserId == Guid.Empty)
            throw new UnauthorizedAccessException();

        var ipAddress = req.IpAddress ?? HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";

        var result = await mediator.Send(req with { IpAddress = ipAddress }, ct);

        await Send.ResponseAsync(ApiResponse<Response>.SuccessResult(result), cancellation: ct);
    }
}

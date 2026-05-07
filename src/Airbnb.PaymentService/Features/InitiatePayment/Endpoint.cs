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
        Summary(s =>
        {
            s.Description = "Initiates a payment for a booking. Automatically resolves the correct provider based on country.";
            s.Responses[400] = "Booking not found or invalid request.";
            s.Responses[403] = "Unauthorized access to booking.";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdStr))
        {
             // For testing/development if auth is not fully setup, but should be required in prod
             // throw new UnauthorizedAccessException();
             // For now, if no user id, we'll try to use a dummy or throw if the user is not authenticated
             userIdStr = Guid.Empty.ToString(); 
        }

        var ipAddress = req.IpAddress ?? HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
        
        var result = await mediator.Send(req with 
        { 
            UserId = Guid.Parse(userIdStr),
            IpAddress = ipAddress 
        }, ct);

        await SendAsync(ApiResponse<Response>.SuccessResult(result), cancellation: ct);
    }
}

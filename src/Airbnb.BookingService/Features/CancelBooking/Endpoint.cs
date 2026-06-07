using Airbnb.ServiceDefaults.Infrastructure;
using FastEndpoints;
using Mediator;

namespace Airbnb.BookingService.Features.CancelBooking;

public class Endpoint(IMediator mediator) : FastEndpoints.Endpoint<Request, ApiResponse<bool>>
{
    public override void Configure()
    {
        Post("/api/bookings/{BookingId}/cancel");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        await mediator.Send(req, ct);
        await Send.ResponseAsync(ApiResponse<bool>.SuccessResult(true), cancellation: ct);
    }
}

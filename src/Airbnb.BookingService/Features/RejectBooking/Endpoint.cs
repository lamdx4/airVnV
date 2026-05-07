using Airbnb.ServiceDefaults.Infrastructure;
using FastEndpoints;
using Mediator;

namespace Airbnb.BookingService.Features.RejectBooking;

public class Endpoint(IMediator mediator) : FastEndpoints.Endpoint<Request, ApiResponse<bool>>
{
    public override void Configure()
    {
        Post("/api/bookings/{BookingId}/reject");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        await mediator.Send(req, ct);
        await SendAsync(ApiResponse<bool>.SuccessResult(true), cancellation: ct);
    }
}

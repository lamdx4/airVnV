using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.BookingService.Features.AdminCancelBooking;

public class Endpoint(IMediator mediator) : FastEndpoints.Endpoint<Request, ApiResponse<bool>>
{
    public override void Configure()
    {
        Patch("/api/bookings/admin/{BookingId}/cancel");
        Roles("Admin", "Moderator");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        await mediator.Send(req, ct);
        await Send.ResponseAsync(ApiResponse<bool>.SuccessResult(true), cancellation: ct);
    }
}

using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.BookingService.Features.GetAdminBookings;

public class Endpoint(IMediator mediator) : FastEndpoints.Endpoint<Request, ApiResponse<AdminBookingListResponse>>
{
    public override void Configure()
    {
        Get("/api/bookings/admin");
        Roles("Admin", "Moderator");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await Send.ResponseAsync(result, cancellation: ct);
    }
}

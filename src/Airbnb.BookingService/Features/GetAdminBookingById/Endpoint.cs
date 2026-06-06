using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.BookingService.Features.GetAdminBookingById;

public class Endpoint(IMediator mediator) : FastEndpoints.Endpoint<Request, ApiResponse<AdminBookingDetailResponse>>
{
    public override void Configure()
    {
        Get("/api/bookings/admin/{Id}");
        Roles("Admin", "Moderator");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await Send.ResponseAsync(result, cancellation: ct);
    }
}

using Airbnb.ServiceDefaults.Infrastructure;
using FastEndpoints;
using Mediator;

namespace Airbnb.BookingService.Features.GetGuestBookings;

public class Endpoint(IMediator mediator) : FastEndpoints.Endpoint<Request, ApiResponse<List<BookingDto>>>
{
    public override void Configure()
    {
        Get("/api/bookings/guest");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await Send.ResponseAsync(ApiResponse<List<BookingDto>>.SuccessResult(result), cancellation: ct);
    }
}

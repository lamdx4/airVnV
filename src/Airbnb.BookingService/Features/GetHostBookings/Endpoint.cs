using Airbnb.ServiceDefaults.Infrastructure;
using FastEndpoints;
using Mediator;

namespace Airbnb.BookingService.Features.GetHostBookings;

public class Endpoint(IMediator mediator) : FastEndpoints.Endpoint<Request, ApiResponse<List<BookingDto>>>
{
    public override void Configure()
    {
        Get("/api/bookings/host");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await SendAsync(ApiResponse<List<BookingDto>>.SuccessResult(result), cancellation: ct);
    }
}

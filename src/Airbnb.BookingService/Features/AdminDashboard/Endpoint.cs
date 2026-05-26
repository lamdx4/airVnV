using Airbnb.BookingService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;
using FastEndpoints;

namespace Airbnb.BookingService.Features.AdminDashboard;

public class Endpoint(BookingDbContext dbContext) : FastEndpoints.Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Get("/api/admin/dashboard/stats");
        Policies("AdminOnly");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var handler = new Handler(dbContext);
        var result = await handler.Handle(req, ct);
        await Send.ResponseAsync(ApiResponse<Response>.SuccessResult(result), cancellation: ct);
    }
}

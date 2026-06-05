using Microsoft.Extensions.Logging;
using Airbnb.UserService.Infrastructure.HttpClients;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Admin.GetRevenueBreakdownReport;

public sealed class Handler(
    BookingServiceClient bookingClient,
    ILogger<Handler> logger)
    : Mediator.IQueryHandler<Request, ApiResponse<List<RevenueBreakdownItem>>>
{
    public async ValueTask<ApiResponse<List<RevenueBreakdownItem>>> Handle(Request req, CancellationToken ct)
    {
        try
        {
            var data = await bookingClient.GetRevenueBreakdownAsync(req.From, req.To, req.GroupBy, ct);
            if (data is null) return ApiResponse<List<RevenueBreakdownItem>>.SuccessResult(new List<RevenueBreakdownItem>());

            var result = data.Select(p => new RevenueBreakdownItem(p.Period, p.Revenue, p.Bookings, p.Cancellations)).ToList();
            return ApiResponse<List<RevenueBreakdownItem>>.SuccessResult(result);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to retrieve revenue breakdown from BookingService");
            return ApiResponse<List<RevenueBreakdownItem>>.SuccessResult(new List<RevenueBreakdownItem>());
        }
    }
}

using Airbnb.ServiceDefaults.Infrastructure;
using Airbnb.UserService.Infrastructure.HttpClients;
using Microsoft.Extensions.Logging;

namespace Airbnb.UserService.Features.Admin.GetRevenueChart;

public sealed class Handler(
    BookingServiceClient bookingClient,
    ILogger<Handler> logger)
    : Mediator.IQueryHandler<Request, ApiResponse<List<RevenueChartPoint>>>
{
    public async ValueTask<ApiResponse<List<RevenueChartPoint>>> Handle(Request req, CancellationToken ct)
    {
        var days = Math.Clamp(req.Days, 1, 365);

        try
        {
            var chartData = await bookingClient.GetRevenueChartAsync(days, ct);
            if (chartData is not null)
            {
                var result = chartData.Select(p => new RevenueChartPoint(p.Date, p.Revenue, p.Bookings)).ToList();
                return ApiResponse<List<RevenueChartPoint>>.SuccessResult(result);
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to retrieve revenue chart from BookingService");
        }

        var empty = Enumerable.Range(0, days)
            .Select(offset => DateTimeOffset.UtcNow.AddDays(-days + offset + 1).Date.ToString("yyyy-MM-dd"))
            .Select(date => new RevenueChartPoint(date, 0, 0))
            .ToList();

        return ApiResponse<List<RevenueChartPoint>>.SuccessResult(empty);
    }
}

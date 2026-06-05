using Microsoft.Extensions.Logging;
using Airbnb.UserService.Infrastructure.HttpClients;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Admin.GetTopPropertiesReport;

public sealed class Handler(
    BookingServiceClient bookingClient,
    PropertyServiceClient propertyClient,
    ILogger<Handler> logger)
    : Mediator.IQueryHandler<Request, ApiResponse<List<TopPropertyItem>>>
{
    public async ValueTask<ApiResponse<List<TopPropertyItem>>> Handle(Request req, CancellationToken ct)
    {
        // 1) Get top properties (without titles) from BookingService
        List<TopPropertyBasic>? basic = null;
        try
        {
            basic = await bookingClient.GetTopPropertiesAsync(req.From, req.To, req.Limit, ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to retrieve top properties from BookingService");
        }

        if (basic is null || basic.Count == 0)
        {
            return ApiResponse<List<TopPropertyItem>>.SuccessResult(new List<TopPropertyItem>());
        }

        // 2) Enrich with titles from PropertyService
        var ids = basic.Select(p => p.PropertyId).ToArray();
        var titleMap = new Dictionary<Guid, string>();
        try
        {
            var properties = await propertyClient.GetPropertiesByIdsAsync(ids, ct);
            if (properties is not null)
            {
                foreach (var p in properties) titleMap[p.Id] = p.Title;
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to retrieve property titles from PropertyService");
        }

        // 3) Merge and return (preserve booking-service ordering)
        var result = basic.Select(p => new TopPropertyItem(
            Id: p.PropertyId,
            Title: titleMap.TryGetValue(p.PropertyId, out var t) ? t : "(unknown)",
            Revenue: p.Revenue,
            Bookings: p.Bookings,
            OccupancyRate: p.OccupancyRate
        )).ToList();

        return ApiResponse<List<TopPropertyItem>>.SuccessResult(result);
    }
}

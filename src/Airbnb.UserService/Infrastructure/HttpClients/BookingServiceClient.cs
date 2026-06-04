using System.Net.Http.Json;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Infrastructure.HttpClients;

public class BookingServiceClient(HttpClient httpClient)
{
    public async Task<BookingStatsResponse?> GetStatsAsync(CancellationToken ct)
    {
        var response = await httpClient.GetAsync("/api/bookings/admin/stats", ct);
        if (!response.IsSuccessStatusCode) return null;

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<BookingStatsResponse>>(cancellationToken: ct);
        return result?.Data;
    }

    public async Task<List<RevenueChartPoint>?> GetRevenueChartAsync(int days, CancellationToken ct)
    {
        var response = await httpClient.GetAsync($"/api/bookings/admin/revenue-chart?days={days}", ct);
        if (!response.IsSuccessStatusCode) return null;

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<RevenueChartPoint>>>(cancellationToken: ct);
        return result?.Data;
    }
}

public record BookingStatsResponse(
    int TotalBookings,
    int ActiveBookings,
    decimal TotalRevenue
);

public record RevenueChartPoint(
    string Date,
    decimal Revenue,
    int Bookings
);

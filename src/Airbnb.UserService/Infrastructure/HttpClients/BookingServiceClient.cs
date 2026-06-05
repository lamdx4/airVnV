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

    // --- UC-E2 Reports additions ---

    public async Task<BookingSummaryResponse?> GetSummaryAsync(DateOnly from, DateOnly to, CancellationToken ct)
    {
        var response = await httpClient.GetAsync(
            $"/api/bookings/admin/summary?from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}", ct);
        if (!response.IsSuccessStatusCode) return null;

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<BookingSummaryResponse>>(cancellationToken: ct);
        return result?.Data;
    }

    public async Task<List<RevenueBreakdownPoint>?> GetRevenueBreakdownAsync(DateOnly from, DateOnly to, string groupBy, CancellationToken ct)
    {
        var response = await httpClient.GetAsync(
            $"/api/bookings/admin/revenue-breakdown?from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}&groupBy={Uri.EscapeDataString(groupBy)}", ct);
        if (!response.IsSuccessStatusCode) return null;

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<RevenueBreakdownPoint>>>(cancellationToken: ct);
        return result?.Data;
    }

    public async Task<List<TopPropertyBasic>?> GetTopPropertiesAsync(DateOnly from, DateOnly to, int limit, CancellationToken ct)
    {
        var response = await httpClient.GetAsync(
            $"/api/bookings/admin/top-properties?from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}&limit={limit}", ct);
        if (!response.IsSuccessStatusCode) return null;

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<TopPropertyBasic>>>(cancellationToken: ct);
        return result?.Data;
    }

    public async Task<OccupancyMetricsResponse?> GetOccupancyMetricsAsync(DateOnly from, DateOnly to, CancellationToken ct)
    {
        var response = await httpClient.GetAsync(
            $"/api/bookings/admin/occupancy?from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}", ct);
        if (!response.IsSuccessStatusCode) return null;

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<OccupancyMetricsResponse>>(cancellationToken: ct);
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

public record BookingSummaryResponse(
    int TotalBookings,
    decimal TotalRevenue,
    decimal AverageBookingValue
);

public record RevenueBreakdownPoint(
    string Period,
    decimal Revenue,
    int Bookings,
    int Cancellations
);

public record TopPropertyBasic(
    Guid PropertyId,
    decimal Revenue,
    int Bookings,
    decimal OccupancyRate
);

public record OccupancyMetricsResponse(
    long BookedNights,
    int RangeDays
);

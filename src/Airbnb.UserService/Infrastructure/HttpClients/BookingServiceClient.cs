using System.Net.Http.Json;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Infrastructure.HttpClients;

public class BookingServiceClient(HttpClient httpClient)
{
    public async Task<BookingSummaryResponse?> GetSummaryAsync(DateOnly from, DateOnly to, CancellationToken ct)
    {
        var response = await httpClient.GetAsync(
            $"/api/bookings/admin/summary?from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}", ct);
        if (!response.IsSuccessStatusCode) return null;

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<BookingSummaryResponse>>(cancellationToken: ct);
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

public record BookingSummaryResponse(
    int TotalBookings,
    decimal TotalRevenue,
    decimal AverageBookingValue
);

public record OccupancyMetricsResponse(
    long BookedNights,
    int RangeDays
);

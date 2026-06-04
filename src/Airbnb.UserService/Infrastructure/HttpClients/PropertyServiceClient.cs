using System.Net.Http.Json;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Infrastructure.HttpClients;

public class PropertyServiceClient(HttpClient httpClient)
{
    public async Task<PropertyStatsResponse?> GetStatsAsync(CancellationToken ct)
    {
        var response = await httpClient.GetAsync("/api/properties/admin/stats", ct);
        if (!response.IsSuccessStatusCode) return null;

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<PropertyStatsResponse>>(cancellationToken: ct);
        return result?.Data;
    }

    public async Task<List<PropertyActivityItem>?> GetRecentActivityAsync(int limit, CancellationToken ct)
    {
        var response = await httpClient.GetAsync($"/api/properties/admin/recent-activity?limit={limit}", ct);
        if (!response.IsSuccessStatusCode) return null;

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<PropertyActivityItem>>>(cancellationToken: ct);
        return result?.Data;
    }
}

public record PropertyStatsResponse(
    int TotalProperties,
    int PendingReview,
    int Published,
    int Suspended,
    int TotalReviews
);

public record PropertyActivityItem(
    Guid Id,
    string Type,
    string Description,
    DateTimeOffset Timestamp
);

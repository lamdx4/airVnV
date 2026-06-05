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

    // --- UC-E2 Reports additions ---

    public async Task<int?> GetPublishedCountAsync(CancellationToken ct)
    {
        var response = await httpClient.GetAsync("/api/properties/admin/published-count", ct);
        if (!response.IsSuccessStatusCode) return null;

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<PublishedCountResponse>>(cancellationToken: ct);
        return result?.Data?.Published;
    }

    public async Task<int?> GetNewPropertiesCountAsync(DateOnly from, DateOnly to, CancellationToken ct)
    {
        var response = await httpClient.GetAsync(
            $"/api/properties/admin/new-count?from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}", ct);
        if (!response.IsSuccessStatusCode) return null;

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<NewPropertiesCountResponse>>(cancellationToken: ct);
        return result?.Data?.NewProperties;
    }

    public async Task<List<PropertyBasicInfo>?> GetPropertiesByIdsAsync(Guid[] ids, CancellationToken ct)
    {
        if (ids is null || ids.Length == 0) return new List<PropertyBasicInfo>();

        var query = string.Join("&", ids.Select(id => $"ids={id}"));
        var response = await httpClient.GetAsync($"/api/properties/admin/by-ids?{query}", ct);
        if (!response.IsSuccessStatusCode) return null;

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<PropertyBasicInfo>>>(cancellationToken: ct);
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

public record PublishedCountResponse(int Published);

public record NewPropertiesCountResponse(int NewProperties);

public record PropertyBasicInfo(Guid Id, string Title);

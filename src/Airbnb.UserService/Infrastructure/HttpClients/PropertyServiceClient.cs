using System.Net.Http.Json;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Infrastructure.HttpClients;

public class PropertyServiceClient(HttpClient httpClient)
{
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

public record PropertyBasicInfo(Guid Id, string Title);

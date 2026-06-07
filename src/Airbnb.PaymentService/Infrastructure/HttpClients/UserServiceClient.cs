using System.Net.Http.Json;

namespace Airbnb.PaymentService.Infrastructure.HttpClients;

public record UserBasicInfoDto(Guid Id, string FullName, string Email, string? AvatarUrl);

public class UserServiceClient(HttpClient httpClient, ILogger<UserServiceClient> logger)
{
    public async Task<Dictionary<Guid, UserBasicInfoDto>> GetBasicInfosAsync(
        IEnumerable<Guid> ids,
        CancellationToken ct = default)
    {
        var idList = ids.Distinct().ToList();
        if (idList.Count == 0) return new();

        try
        {
            var resp = await httpClient.PostAsJsonAsync(
                "/api/internal/users/basic-infos",
                new { Ids = idList },
                ct);

            if (!resp.IsSuccessStatusCode)
            {
                logger.LogWarning("UserService basic-infos returned {Status}", resp.StatusCode);
                return new();
            }

            var wrapper = await resp.Content.ReadFromJsonAsync<ApiResponseWrapper<UserBasicInfosResponse>>(ct);
            return wrapper?.Data?.Items?.ToDictionary(u => u.Id) ?? new();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to call UserService basic-infos. Continuing without enrichment.");
            return new();
        }
    }

    public async Task<List<Guid>> GetSampleHostIdsAsync(int count = 3, CancellationToken ct = default)
    {
        try
        {
            var resp = await httpClient.GetAsync($"/api/internal/users/sample-hosts?count={count}", ct);
            if (!resp.IsSuccessStatusCode) return new();
            var wrapper = await resp.Content.ReadFromJsonAsync<ApiResponseWrapper<SampleHostsResponse>>(ct);
            return wrapper?.Data?.Items?.Select(h => h.Id).ToList() ?? new();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to call UserService sample-hosts.");
            return new();
        }
    }

    private record UserBasicInfosResponse(List<UserBasicInfoDto> Items);
    private record SampleHostsResponse(List<HostStub> Items);
    private record HostStub(Guid Id, string FullName);
}

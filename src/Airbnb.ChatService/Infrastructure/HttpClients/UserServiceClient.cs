using System.Net.Http.Json;

namespace Airbnb.ChatService.Infrastructure.HttpClients;

public record UserPublicProfileResponse(Guid Id, string FullName, string? AvatarUrl);

public class UserServiceClient(HttpClient httpClient)
{
    public async Task<UserPublicProfileResponse?> GetPublicProfileAsync(Guid userId, CancellationToken ct = default)
    {
        var wrapper = await httpClient.GetFromJsonAsync<ApiResponseWrapper<UserPublicProfileResponse>>($"/api/users/{userId}/public-profile", ct);
        return wrapper?.Data;
    }
}

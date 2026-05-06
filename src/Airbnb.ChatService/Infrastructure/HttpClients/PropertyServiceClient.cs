using System.Net.Http.Json;

namespace Airbnb.ChatService.Infrastructure.HttpClients;

public record PropertyBasicInfoResponse(Guid PropertyId, string Title, Guid HostId);

public class PropertyServiceClient(HttpClient httpClient)
{
    public async Task<PropertyBasicInfoResponse?> GetPropertyBasicInfoAsync(Guid propertyId, CancellationToken ct = default)
    {
        // Giao tiếp qua YARP hoặc Service Discovery (được cấu hình trong Program.cs)
        // Bọc Response vì PropertyService trả về ApiResponse<T>
        var wrapper = await httpClient.GetFromJsonAsync<ApiResponseWrapper<PropertyBasicInfoResponse>>($"/api/properties/{propertyId}/basic-info", ct);
        return wrapper?.Data;
    }
}

public record ApiResponseWrapper<T>(T? Data, string? Message, bool Success, string? ErrorCode);

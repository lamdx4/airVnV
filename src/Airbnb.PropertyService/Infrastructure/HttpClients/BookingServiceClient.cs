using System.Net.Http.Json;

namespace Airbnb.PropertyService.Infrastructure.HttpClients;

public class BookingServiceClient(HttpClient httpClient)
{
    public async Task<BookingValidationResponse?> GetBookingAsync(Guid bookingId, CancellationToken ct)
    {
        try
        {
            var response = await httpClient.GetAsync($"/api/bookings/{bookingId}/basic-info", ct);
            if (!response.IsSuccessStatusCode)
                return null;
                
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<BookingValidationResponse>>(cancellationToken: ct);
            return result?.Data;
        }
        catch
        {
            return null;
        }
    }
}

public record BookingValidationResponse(Guid Id, Guid GuestId, string Status);

public record ApiResponse<T>(
    T? Data,
    string? Message = null,
    bool Success = true,
    string? ErrorCode = null,
    List<string>? Errors = null);

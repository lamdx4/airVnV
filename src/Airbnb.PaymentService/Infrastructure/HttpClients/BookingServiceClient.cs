using System.Net.Http.Json;

namespace Airbnb.PaymentService.Infrastructure.HttpClients;

public record BookingBasicInfoResponse(Guid BookingId, decimal TotalPrice, string CurrencyCode, string CountryCode, Guid GuestId);

public class BookingServiceClient(HttpClient httpClient)
{
    public async Task<BookingBasicInfoResponse?> GetBookingBasicInfoAsync(Guid bookingId, CancellationToken ct = default)
    {
        var wrapper = await httpClient.GetFromJsonAsync<ApiResponseWrapper<BookingBasicInfoResponse>>($"/api/internal/bookings/{bookingId}", ct);
        return wrapper?.Data;
    }
}

public record ApiResponseWrapper<T>(T? Data, string? Message, bool Success, string? ErrorCode);

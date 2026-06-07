using System.Net.Http.Json;

namespace Airbnb.PaymentService.Infrastructure.HttpClients;

public record BookingBasicInfoResponse(Guid BookingId, decimal TotalPrice, string CurrencyCode, string CountryCode, Guid GuestId);

public class BookingServiceClient(HttpClient httpClient, ILogger<BookingServiceClient> logger)
{
    public async Task<BookingBasicInfoResponse?> GetBookingBasicInfoAsync(Guid bookingId, CancellationToken ct = default)
    {
        var wrapper = await httpClient.GetFromJsonAsync<ApiResponseWrapper<BookingBasicInfoResponse>>($"/api/internal/bookings/{bookingId}", ct);
        return wrapper?.Data;
    }

    public async Task<Dictionary<Guid, BookingBasicInfoResponse>> GetBasicInfosAsync(
        IEnumerable<Guid> bookingIds,
        CancellationToken ct = default)
    {
        var ids = bookingIds.Distinct().ToList();
        if (ids.Count == 0) return new();

        try
        {
            var resp = await httpClient.PostAsJsonAsync(
                "/api/internal/bookings/basic-infos",
                new { Ids = ids },
                ct);

            if (!resp.IsSuccessStatusCode)
            {
                logger.LogWarning("BookingService basic-infos returned {Status}", resp.StatusCode);
                return new();
            }

            var wrapper = await resp.Content.ReadFromJsonAsync<ApiResponseWrapper<BookingBasicInfosResponse>>(ct);
            return wrapper?.Data?.Items?.ToDictionary(b => b.BookingId) ?? new();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to call BookingService basic-infos. Continuing without enrichment.");
            return new();
        }
    }

    private record BookingBasicInfosResponse(List<BookingBasicInfoResponse> Items);
}

public record ApiResponseWrapper<T>(T? Data, string? Message, bool Success, string? ErrorCode);

using Airbnb.PaymentService.Domain;
using Microsoft.AspNetCore.Http;

namespace Airbnb.PaymentService.Infrastructure.PaymentGateways;

public record PaymentUrlResult(string Url, DateTimeOffset ExpiresAt);

public record WebhookResult(bool IsSuccess, Guid PaymentId, string? TransactionNo, string? ResponseCode, string? Message);

public record WebhookRequest(
    string RawBody,
    IHeaderDictionary Headers,
    IQueryCollection QueryParams
);

public interface IPaymentProvider
{
    string ProviderName { get; }
    Task<PaymentUrlResult> GeneratePaymentUrlAsync(Payment payment, string ipAddress, CancellationToken ct);
    Task<WebhookResult> ProcessWebhookAsync(WebhookRequest request, CancellationToken ct);
}

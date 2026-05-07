using FastEndpoints;
using Mediator;
using Airbnb.PaymentService.Infrastructure.PaymentGateways;

namespace Airbnb.PaymentService.Features.VnpayIpn;

public class Endpoint(
    IMediator mediator, 
    IEnumerable<IPaymentProvider> providers, 
    ILogger<Endpoint> logger)
    : FastEndpoints.EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/api/payments/vnpay/ipn");
        AllowAnonymous();
        Summary(s =>
        {
            s.Description = "VNPay IPN URL to receive payment";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        // 1. Resolve VNPay Provider
        var provider = providers.FirstOrDefault(p => p.ProviderName == "vnpay");
        if (provider == null)
        {
            logger.LogCritical("VnpayProvider not found for IPN processing");
            await SendErrorsAsync(500, ct);
            return;
        }

        // 2. Map Request to Generic WebhookRequest
        var webhookRequest = new WebhookRequest(
            string.Empty, // GET has no body
            HttpContext.Request.Headers,
            HttpContext.Request.Query
        );

        // 3. Delegate to Provider
        var result = await provider.ProcessWebhookAsync(webhookRequest, ct);

        logger.LogInformation("VNPay IPN processed. Success: {Success}, Txn: {Txn}", result.IsSuccess, result.TransactionNo);

        if (result.IsSuccess)
        {
            await mediator.Send(new ConfirmPayment.Command(result.PaymentId, result.TransactionNo!), ct);
            await SendAsync(new { RspCode = "00", Message = result.Message }, cancellation: ct);
        }
        else
        {
            // Publish failure event for Saga via Mediator Command
            await mediator.Send(new FailPayment.Command(result.PaymentId, result.ResponseCode, result.Message), ct);
            
            // Even on failure (logic wise), we must return the RspCode VNPay expects
            var rspCode = result.ResponseCode ?? "99";
            await SendAsync(new { RspCode = rspCode, Message = result.Message }, cancellation: ct);
        }
    }
}

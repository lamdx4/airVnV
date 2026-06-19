using Airbnb.PaymentService.Domain;
using Microsoft.Extensions.Configuration;

namespace Airbnb.PaymentService.Infrastructure.PaymentGateways;

public class VnpayProvider(IConfiguration config, ILogger<VnpayProvider> logger) : IPaymentProvider
{
    public string ProviderName => "vnpay";

    public Task<PaymentUrlResult> GeneratePaymentUrlAsync(Payment payment, string ipAddress, CancellationToken ct)
    {
        logger.LogInformation("Generating VNPay payment URL for booking {BookingId}", payment.BookingId);
        var vnpay = new VnpayLibrary();
        var vnp_TmnCode = config["Vnpay:TmnCode"];
        var vnp_HashSecret = config["Vnpay:HashSecret"];
        var vnp_Url = config["Vnpay:Url"];
        var frontendUrl = config["FrontendUrl"] ?? "http://localhost:5173";
        var vnp_ReturnUrl = $"{frontendUrl.TrimEnd('/')}/payment/callback";

        vnpay.AddRequestData("vnp_Version", "2.1.0");
        vnpay.AddRequestData("vnp_Command", "pay");
        vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode!);
        vnpay.AddRequestData("vnp_Amount", ((long)(payment.Amount * 100)).ToString());
        var vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById(OperatingSystem.IsWindows() ? "SE Asia Standard Time" : "Asia/Ho_Chi_Minh");
        var vnNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vnTimeZone);
        vnpay.AddRequestData("vnp_CreateDate", vnNow.ToString("yyyyMMddHHmmss"));
        vnpay.AddRequestData("vnp_CurrCode", payment.Currency);
        vnpay.AddRequestData("vnp_IpAddr", ipAddress);
        vnpay.AddRequestData("vnp_Locale", "vn");
        vnpay.AddRequestData("vnp_OrderInfo", $"Thanh toan booking {payment.BookingId}");
        vnpay.AddRequestData("vnp_OrderType", "other");
        vnpay.AddRequestData("vnp_ReturnUrl", vnp_ReturnUrl);
        vnpay.AddRequestData("vnp_TxnRef", payment.Id.ToString());

        var paymentUrl = vnpay.CreateRequestUrl(vnp_Url!, vnp_HashSecret!);
        
        // VNPay standard expiry is 15 minutes
        return Task.FromResult(new PaymentUrlResult(paymentUrl, DateTimeOffset.UtcNow.AddMinutes(15)));
    }

    public Task<WebhookResult> ProcessWebhookAsync(WebhookRequest request, CancellationToken ct)
    {
        var vnpay = new VnpayLibrary();
        foreach (var query in request.QueryParams)
        {
            vnpay.AddResponseData(query.Key, query.Value!);
        }

        var vnp_SecureHash = request.QueryParams["vnp_SecureHash"];
        var hashSecret = config["Vnpay:HashSecret"];

        bool isValid = vnpay.ValidateSignature(vnp_SecureHash!, hashSecret!);

        if (!isValid)
        {
            return Task.FromResult(new WebhookResult(false, Guid.Empty, null, "97", "Invalid Checksum"));
        }

        var vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
        var vnp_TransactionNo = vnpay.GetResponseData("vnp_TransactionNo");
        var vnp_TxnRef = vnpay.GetResponseData("vnp_TxnRef");

        if (vnp_ResponseCode == "00")
        {
            return Task.FromResult(new WebhookResult(true, Guid.Parse(vnp_TxnRef), vnp_TransactionNo, vnp_ResponseCode, "Confirm Success"));
        }

        return Task.FromResult(new WebhookResult(false, Guid.Parse(vnp_TxnRef), vnp_TransactionNo, vnp_ResponseCode, "Payment Failed"));
    }
}

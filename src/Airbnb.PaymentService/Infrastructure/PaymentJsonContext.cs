using System.Text.Json.Serialization;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Infrastructure;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(Airbnb.PaymentService.Features.InitiatePayment.Request), TypeInfoPropertyName = "InitiatePaymentRequest")]
[JsonSerializable(typeof(Airbnb.PaymentService.Features.InitiatePayment.Response), TypeInfoPropertyName = "InitiatePaymentResponse")]
[JsonSerializable(typeof(FastEndpoints.ErrorResponse))]
// Admin Payments
[JsonSerializable(typeof(Airbnb.PaymentService.Features.GetAdminPayments.Request), TypeInfoPropertyName = "GetAdminPaymentsRequest")]
[JsonSerializable(typeof(Airbnb.PaymentService.Features.GetAdminPayments.AdminPaymentResponse))]
[JsonSerializable(typeof(Airbnb.PaymentService.Features.GetAdminPayments.PagedResponse<Airbnb.PaymentService.Features.GetAdminPayments.AdminPaymentResponse>))]
[JsonSerializable(typeof(ApiResponse<Airbnb.PaymentService.Features.GetAdminPayments.PagedResponse<Airbnb.PaymentService.Features.GetAdminPayments.AdminPaymentResponse>>))]
[JsonSerializable(typeof(Airbnb.PaymentService.Features.GetAdminPaymentDetail.AdminPaymentDetailResponse))]
[JsonSerializable(typeof(Airbnb.PaymentService.Features.GetAdminPaymentDetail.RefundRecordDto))]
[JsonSerializable(typeof(ApiResponse<Airbnb.PaymentService.Features.GetAdminPaymentDetail.AdminPaymentDetailResponse>))]
[JsonSerializable(typeof(Airbnb.PaymentService.Features.RefundPayment.RefundPaymentResponse))]
[JsonSerializable(typeof(ApiResponse<Airbnb.PaymentService.Features.RefundPayment.RefundPaymentResponse>))]
// Admin Payouts
[JsonSerializable(typeof(Airbnb.PaymentService.Features.GetAdminPayouts.AdminPayoutResponse))]
[JsonSerializable(typeof(Airbnb.PaymentService.Features.GetAdminPayouts.PagedResponse<Airbnb.PaymentService.Features.GetAdminPayouts.AdminPayoutResponse>))]
[JsonSerializable(typeof(ApiResponse<Airbnb.PaymentService.Features.GetAdminPayouts.PagedResponse<Airbnb.PaymentService.Features.GetAdminPayouts.AdminPayoutResponse>>))]
[JsonSerializable(typeof(Airbnb.PaymentService.Features.GetAdminPayoutDetail.AdminPayoutDetailResponse))]
[JsonSerializable(typeof(Airbnb.PaymentService.Features.GetAdminPayoutDetail.PayoutItemDto))]
[JsonSerializable(typeof(ApiResponse<Airbnb.PaymentService.Features.GetAdminPayoutDetail.AdminPayoutDetailResponse>))]
[JsonSerializable(typeof(Airbnb.PaymentService.Features.GeneratePayouts.GeneratePayoutsResponse))]
[JsonSerializable(typeof(ApiResponse<Airbnb.PaymentService.Features.GeneratePayouts.GeneratePayoutsResponse>))]
// Admin Platform Fee
[JsonSerializable(typeof(Airbnb.PaymentService.Features.GetAdminPlatformFeeCurrent.PlatformFeeCurrentResponse))]
[JsonSerializable(typeof(ApiResponse<Airbnb.PaymentService.Features.GetAdminPlatformFeeCurrent.PlatformFeeCurrentResponse>))]
[JsonSerializable(typeof(Airbnb.PaymentService.Features.GetAdminPlatformFeeHistory.PlatformFeeHistoryItem))]
[JsonSerializable(typeof(Airbnb.PaymentService.Features.GetAdminPlatformFeeHistory.PagedResponse<Airbnb.PaymentService.Features.GetAdminPlatformFeeHistory.PlatformFeeHistoryItem>))]
[JsonSerializable(typeof(ApiResponse<Airbnb.PaymentService.Features.GetAdminPlatformFeeHistory.PagedResponse<Airbnb.PaymentService.Features.GetAdminPlatformFeeHistory.PlatformFeeHistoryItem>>))]
[JsonSerializable(typeof(Airbnb.PaymentService.Features.CreateAdminPlatformFee.PlatformFeeCreateResponse))]
[JsonSerializable(typeof(ApiResponse<Airbnb.PaymentService.Features.CreateAdminPlatformFee.PlatformFeeCreateResponse>))]
internal partial class PaymentJsonContext : JsonSerializerContext { }

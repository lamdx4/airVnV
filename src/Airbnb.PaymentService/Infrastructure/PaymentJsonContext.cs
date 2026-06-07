using System.Text.Json.Serialization;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Infrastructure;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(Airbnb.PaymentService.Features.InitiatePayment.Request), TypeInfoPropertyName = "InitiatePaymentRequest")]
[JsonSerializable(typeof(Airbnb.PaymentService.Features.InitiatePayment.Response), TypeInfoPropertyName = "InitiatePaymentResponse")]
[JsonSerializable(typeof(FastEndpoints.ErrorResponse))]
internal partial class PaymentJsonContext : JsonSerializerContext { }

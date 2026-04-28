using System.Text.Json.Serialization;
using Airbnb.PaymentService.Features.ProcessPayment;

namespace Airbnb.PaymentService.Infrastructure;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(Request))]
[JsonSerializable(typeof(Response))]
[JsonSerializable(typeof(FastEndpoints.ErrorResponse))]
internal partial class PaymentJsonContext : JsonSerializerContext { }

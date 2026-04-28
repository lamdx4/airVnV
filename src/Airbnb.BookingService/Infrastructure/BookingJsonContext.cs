using System.Text.Json.Serialization;
using Airbnb.BookingService.Features.CreateBooking;

namespace Airbnb.BookingService.Infrastructure;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(Request))]
[JsonSerializable(typeof(Response))]
[JsonSerializable(typeof(FastEndpoints.ErrorResponse))]
internal partial class BookingJsonContext : JsonSerializerContext { }

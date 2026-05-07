using System.Text.Json.Serialization;
using Airbnb.BookingService.Features.CreateBooking;
using Airbnb.BookingService.Features.GetBookingBasicInfo;

namespace Airbnb.BookingService.Infrastructure;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(Airbnb.BookingService.Features.CreateBooking.Request))]
[JsonSerializable(typeof(Airbnb.BookingService.Features.CreateBooking.Response))]
[JsonSerializable(typeof(Airbnb.BookingService.Features.GetBookingBasicInfo.Request))]
[JsonSerializable(typeof(Airbnb.BookingService.Features.GetBookingBasicInfo.Response))]
[JsonSerializable(typeof(FastEndpoints.ErrorResponse))]
internal partial class BookingJsonContext : JsonSerializerContext { }

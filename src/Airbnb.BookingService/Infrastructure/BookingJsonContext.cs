using System.Text.Json.Serialization;
using Airbnb.BookingService.Features.CreateBooking;
using Airbnb.BookingService.Features.GetBookingBasicInfo;

namespace Airbnb.BookingService.Infrastructure;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(Airbnb.BookingService.Features.CreateBooking.Request), TypeInfoPropertyName = "CreateBookingRequest")]
[JsonSerializable(typeof(Airbnb.BookingService.Features.CreateBooking.Response), TypeInfoPropertyName = "CreateBookingResponse")]
[JsonSerializable(typeof(Airbnb.BookingService.Features.GetBookingBasicInfo.Request), TypeInfoPropertyName = "GetBookingBasicInfoRequest")]
[JsonSerializable(typeof(Airbnb.BookingService.Features.GetBookingBasicInfo.Response), TypeInfoPropertyName = "GetBookingBasicInfoResponse")]
[JsonSerializable(typeof(Airbnb.BookingService.Features.GetAdminStats.BookingStatsResponse))]
[JsonSerializable(typeof(Airbnb.ServiceDefaults.Infrastructure.ApiResponse<Airbnb.BookingService.Features.GetAdminStats.BookingStatsResponse>))]
[JsonSerializable(typeof(Airbnb.BookingService.Features.GetRevenueChart.RevenueChartPoint))]
[JsonSerializable(typeof(Airbnb.ServiceDefaults.Infrastructure.ApiResponse<List<Airbnb.BookingService.Features.GetRevenueChart.RevenueChartPoint>>))]
[JsonSerializable(typeof(FastEndpoints.ErrorResponse))]
internal partial class BookingJsonContext : JsonSerializerContext { }

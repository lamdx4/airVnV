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

// --- Admin: GetBookingSummary (UC-E2 Reports) ---
[JsonSerializable(typeof(Airbnb.BookingService.Features.GetBookingSummary.Request), TypeInfoPropertyName = "AdminGetBookingSummaryRequest")]
[JsonSerializable(typeof(Airbnb.BookingService.Features.GetBookingSummary.BookingSummaryResponse))]
[JsonSerializable(typeof(Airbnb.ServiceDefaults.Infrastructure.ApiResponse<Airbnb.BookingService.Features.GetBookingSummary.BookingSummaryResponse>))]

// --- Admin: GetRevenueBreakdown (UC-E2 Reports) ---
[JsonSerializable(typeof(Airbnb.BookingService.Features.GetRevenueBreakdown.Request), TypeInfoPropertyName = "AdminGetRevenueBreakdownRequest")]
[JsonSerializable(typeof(Airbnb.BookingService.Features.GetRevenueBreakdown.RevenueBreakdownPoint))]
[JsonSerializable(typeof(Airbnb.ServiceDefaults.Infrastructure.ApiResponse<List<Airbnb.BookingService.Features.GetRevenueBreakdown.RevenueBreakdownPoint>>))]

// --- Admin: GetTopPropertiesAdmin (UC-E2 Reports) ---
[JsonSerializable(typeof(Airbnb.BookingService.Features.GetTopPropertiesAdmin.Request), TypeInfoPropertyName = "AdminGetTopPropertiesRequest")]
[JsonSerializable(typeof(Airbnb.BookingService.Features.GetTopPropertiesAdmin.TopPropertyBasic))]
[JsonSerializable(typeof(Airbnb.ServiceDefaults.Infrastructure.ApiResponse<List<Airbnb.BookingService.Features.GetTopPropertiesAdmin.TopPropertyBasic>>))]

// --- Admin: GetOccupancyMetrics (UC-E2 Reports) ---
[JsonSerializable(typeof(Airbnb.BookingService.Features.GetOccupancyMetrics.Request), TypeInfoPropertyName = "AdminGetOccupancyMetricsRequest")]
[JsonSerializable(typeof(Airbnb.BookingService.Features.GetOccupancyMetrics.OccupancyMetricsResponse))]
[JsonSerializable(typeof(Airbnb.ServiceDefaults.Infrastructure.ApiResponse<Airbnb.BookingService.Features.GetOccupancyMetrics.OccupancyMetricsResponse>))]

[JsonSerializable(typeof(FastEndpoints.ErrorResponse))]
internal partial class BookingJsonContext : JsonSerializerContext { }

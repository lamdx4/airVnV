using System.Text.Json.Serialization;
using Airbnb.BookingService.Features.CreateBooking;
using Airbnb.BookingService.Features.GetBookingBasicInfo;
using Airbnb.BookingService.Features.GetBookingBasicInfos;

namespace Airbnb.BookingService.Infrastructure;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(Airbnb.BookingService.Features.CreateBooking.Request), TypeInfoPropertyName = "CreateBookingRequest")]
[JsonSerializable(typeof(Airbnb.BookingService.Features.CreateBooking.Response), TypeInfoPropertyName = "CreateBookingResponse")]
[JsonSerializable(typeof(Airbnb.ServiceDefaults.Infrastructure.ApiResponse<Airbnb.BookingService.Features.CreateBooking.Response>), TypeInfoPropertyName = "ApiResponseCreateBooking")]
[JsonSerializable(typeof(Airbnb.BookingService.Features.GetBookingBasicInfo.Request), TypeInfoPropertyName = "GetBookingBasicInfoRequest")]
[JsonSerializable(typeof(Airbnb.BookingService.Features.GetBookingBasicInfo.Response), TypeInfoPropertyName = "GetBookingBasicInfoResponse")]
[JsonSerializable(typeof(Airbnb.ServiceDefaults.Infrastructure.ApiResponse<Airbnb.BookingService.Features.GetBookingBasicInfo.Response>), TypeInfoPropertyName = "ApiResponseGetBookingBasicInfo")]
[JsonSerializable(typeof(Airbnb.BookingService.Features.GetBookingBasicInfos.Request), TypeInfoPropertyName = "GetBookingBasicInfosRequest")]
[JsonSerializable(typeof(Airbnb.BookingService.Features.GetBookingBasicInfos.Response), TypeInfoPropertyName = "GetBookingBasicInfosResponse")]
[JsonSerializable(typeof(Airbnb.ServiceDefaults.Infrastructure.ApiResponse<Airbnb.BookingService.Features.GetBookingBasicInfos.Response>), TypeInfoPropertyName = "ApiResponseGetBookingBasicInfos")]
[JsonSerializable(typeof(Airbnb.ServiceDefaults.Infrastructure.ApiResponse<bool>))]
[JsonSerializable(typeof(Airbnb.BookingService.Features.CancelBooking.Request), TypeInfoPropertyName = "CancelBookingRequest")]
[JsonSerializable(typeof(Airbnb.BookingService.Features.ApproveBooking.Request), TypeInfoPropertyName = "ApproveBookingRequest")]
[JsonSerializable(typeof(Airbnb.BookingService.Features.RejectBooking.Request), TypeInfoPropertyName = "RejectBookingRequest")]
[JsonSerializable(typeof(Airbnb.BookingService.Features.GetGuestBookings.BookingDto), TypeInfoPropertyName = "GuestBookingDto")]
[JsonSerializable(typeof(List<Airbnb.BookingService.Features.GetGuestBookings.BookingDto>), TypeInfoPropertyName = "ListGuestBookingDto")]
[JsonSerializable(typeof(Airbnb.ServiceDefaults.Infrastructure.ApiResponse<List<Airbnb.BookingService.Features.GetGuestBookings.BookingDto>>), TypeInfoPropertyName = "ApiResponseGuestBookings")]
[JsonSerializable(typeof(Airbnb.BookingService.Features.GetHostBookings.BookingDto), TypeInfoPropertyName = "HostBookingDto")]
[JsonSerializable(typeof(List<Airbnb.BookingService.Features.GetHostBookings.BookingDto>), TypeInfoPropertyName = "ListHostBookingDto")]
[JsonSerializable(typeof(Airbnb.ServiceDefaults.Infrastructure.ApiResponse<List<Airbnb.BookingService.Features.GetHostBookings.BookingDto>>), TypeInfoPropertyName = "ApiResponseHostBookings")]
[JsonSerializable(typeof(FastEndpoints.ErrorResponse))]
internal partial class BookingJsonContext : JsonSerializerContext { }

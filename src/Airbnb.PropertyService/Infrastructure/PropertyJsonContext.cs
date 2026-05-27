using System.Text.Json.Serialization;
using Airbnb.PropertyService.Features.CreateProperty;

namespace Airbnb.PropertyService.Infrastructure;

[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
[JsonSerializable(typeof(Airbnb.PropertyService.Features.CreateProperty.Request), TypeInfoPropertyName = "CreatePropertyRequest")]
[JsonSerializable(typeof(Airbnb.PropertyService.Features.CreateProperty.CreatePropertyDto), TypeInfoPropertyName = "CreatePropertyDto")]
[JsonSerializable(typeof(Airbnb.PropertyService.Features.CreateProperty.Response), TypeInfoPropertyName = "CreatePropertyResponse")]
[JsonSerializable(typeof(Airbnb.PropertyService.Features.GetProperty.Request), TypeInfoPropertyName = "GetPropertyRequest")]
[JsonSerializable(typeof(Airbnb.ServiceDefaults.Infrastructure.ApiResponse<Airbnb.PropertyService.Features.GetProperty.PropertyDto>), TypeInfoPropertyName = "ApiResponseGetProperty")]

[JsonSerializable(typeof(Airbnb.PropertyService.Domain.ValueObjects.AddressRaw), TypeInfoPropertyName = "AddressRaw")]
[JsonSerializable(typeof(Airbnb.PropertyService.Domain.ValueObjects.AddressNotes), TypeInfoPropertyName = "AddressNotes")]
[JsonSerializable(typeof(Airbnb.PropertyService.Domain.ValueObjects.HouseRules), TypeInfoPropertyName = "HouseRules")]
[JsonSerializable(typeof(System.Collections.Generic.List<string>), TypeInfoPropertyName = "StringList")]
[JsonSerializable(typeof(Dictionary<string, string>), TypeInfoPropertyName = "StringDictionary")]


[JsonSerializable(typeof(Airbnb.PropertyService.Features.GetMyProperties.Request), TypeInfoPropertyName = "GetMyPropertiesRequest")]
[JsonSerializable(typeof(Airbnb.PropertyService.Features.GetMyProperties.PropertyResponse), TypeInfoPropertyName = "GetMyPropertiesPropertyResponse")]
[JsonSerializable(typeof(Airbnb.PropertyService.Features.GetMyProperties.PagedResponse<Airbnb.PropertyService.Features.GetMyProperties.PropertyResponse>), TypeInfoPropertyName = "GetMyPropertiesPagedResponse")]
[JsonSerializable(typeof(Airbnb.ServiceDefaults.Infrastructure.ApiResponse<Airbnb.PropertyService.Features.GetMyProperties.PagedResponse<Airbnb.PropertyService.Features.GetMyProperties.PropertyResponse>>), TypeInfoPropertyName = "ApiResponseGetMyPropertiesPaged")]

// AddReview
[JsonSerializable(typeof(Airbnb.PropertyService.Features.AddReview.Request))]
[JsonSerializable(typeof(Airbnb.PropertyService.Features.AddReview.Response))]
[JsonSerializable(typeof(Airbnb.ServiceDefaults.Infrastructure.ApiResponse<Airbnb.PropertyService.Features.AddReview.Response>))]
[JsonSerializable(typeof(Airbnb.PropertyService.Infrastructure.HttpClients.BookingValidationResponse))]
[JsonSerializable(typeof(Airbnb.PropertyService.Infrastructure.HttpClients.ApiResponse<Airbnb.PropertyService.Infrastructure.HttpClients.BookingValidationResponse>))]

[JsonSerializable(typeof(Airbnb.PropertyService.Features.UpdateReview.UpdateReviewRequest))]
[JsonSerializable(typeof(Airbnb.PropertyService.Features.UpdateReview.UpdateReviewResponse))]
[JsonSerializable(typeof(Airbnb.ServiceDefaults.Infrastructure.ApiResponse<Airbnb.PropertyService.Features.UpdateReview.UpdateReviewResponse>))]

[JsonSerializable(typeof(Airbnb.PropertyService.Features.DeleteReview.DeleteReviewRequest))]
[JsonSerializable(typeof(Airbnb.PropertyService.Features.DeleteReview.DeleteReviewResponse))]
[JsonSerializable(typeof(Airbnb.ServiceDefaults.Infrastructure.ApiResponse<Airbnb.PropertyService.Features.DeleteReview.DeleteReviewResponse>))]

[JsonSerializable(typeof(Airbnb.PropertyService.Features.GetReviews.GetReviewsRequest))]
[JsonSerializable(typeof(Airbnb.PropertyService.Features.GetReviews.GetReviewsResponse))]
[JsonSerializable(typeof(Airbnb.ServiceDefaults.Infrastructure.ApiResponse<Airbnb.PropertyService.Features.GetReviews.GetReviewsResponse>))]

[JsonSerializable(typeof(FastEndpoints.ErrorResponse))] // Quan trọng for validation errors
[JsonSerializable(typeof(Airbnb.PropertyService.Features.AdminPropertyModeration.GetPendingProperties.Request), TypeInfoPropertyName = "GetPendingPropertiesRequest")]
[JsonSerializable(typeof(Airbnb.PropertyService.Features.AdminPropertyModeration.GetPendingProperties.Response), TypeInfoPropertyName = "GetPendingPropertiesResponse")]
[JsonSerializable(typeof(Airbnb.ServiceDefaults.Infrastructure.ApiResponse<Airbnb.PropertyService.Features.AdminPropertyModeration.GetPendingProperties.Response>), TypeInfoPropertyName = "ApiResponseGetPendingProperties")]

[JsonSerializable(typeof(Airbnb.PropertyService.Features.AdminPropertyModeration.RejectProperty.Request), TypeInfoPropertyName = "RejectPropertyRequest")]
[JsonSerializable(typeof(Airbnb.PropertyService.Features.AdminPropertyModeration.RejectProperty.Response), TypeInfoPropertyName = "RejectPropertyResponse")]
[JsonSerializable(typeof(Airbnb.ServiceDefaults.Infrastructure.ApiResponse<Airbnb.PropertyService.Features.AdminPropertyModeration.RejectProperty.Response>), TypeInfoPropertyName = "ApiResponseRejectProperty")]

[JsonSerializable(typeof(FastEndpoints.ErrorResponse))] // Quan trọng cho validation errors
internal partial class PropertyJsonContext : JsonSerializerContext { }

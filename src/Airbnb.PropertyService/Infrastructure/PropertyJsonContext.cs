using System.Text.Json.Serialization;
using Airbnb.PropertyService.Features.CreateProperty;

namespace Airbnb.PropertyService.Infrastructure;

[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
[JsonSerializable(typeof(Airbnb.PropertyService.Features.CreateProperty.Request), TypeInfoPropertyName = "CreatePropertyRequest")]
[JsonSerializable(typeof(Airbnb.PropertyService.Features.CreateProperty.CreatePropertyDto), TypeInfoPropertyName = "CreatePropertyDto")]
[JsonSerializable(typeof(Airbnb.PropertyService.Features.CreateProperty.ImageMetadataDto), TypeInfoPropertyName = "ImageMetadataDto")]
[JsonSerializable(typeof(System.Collections.Generic.List<Airbnb.PropertyService.Features.CreateProperty.ImageMetadataDto>), TypeInfoPropertyName = "ImageMetadataDtoList")]
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

// GetAdminStats
[JsonSerializable(typeof(Airbnb.PropertyService.Features.GetAdminStats.PropertyStatsResponse))]
[JsonSerializable(typeof(Airbnb.ServiceDefaults.Infrastructure.ApiResponse<Airbnb.PropertyService.Features.GetAdminStats.PropertyStatsResponse>))]

// GetRecentActivity
[JsonSerializable(typeof(Airbnb.PropertyService.Features.GetRecentActivity.ActivityItem))]
[JsonSerializable(typeof(Airbnb.ServiceDefaults.Infrastructure.ApiResponse<List<Airbnb.PropertyService.Features.GetRecentActivity.ActivityItem>>))]

// --- Admin: GetPublishedCount (UC-E2 Reports) ---
[JsonSerializable(typeof(Airbnb.PropertyService.Features.GetPublishedCount.Request), TypeInfoPropertyName = "AdminGetPublishedCountRequest")]
[JsonSerializable(typeof(Airbnb.PropertyService.Features.GetPublishedCount.PublishedCountResponse))]
[JsonSerializable(typeof(Airbnb.ServiceDefaults.Infrastructure.ApiResponse<Airbnb.PropertyService.Features.GetPublishedCount.PublishedCountResponse>))]

// --- Admin: GetNewPropertiesCount (UC-E2 Reports) ---
[JsonSerializable(typeof(Airbnb.PropertyService.Features.GetNewPropertiesCount.Request), TypeInfoPropertyName = "AdminGetNewPropertiesCountRequest")]
[JsonSerializable(typeof(Airbnb.PropertyService.Features.GetNewPropertiesCount.NewPropertiesCountResponse))]
[JsonSerializable(typeof(Airbnb.ServiceDefaults.Infrastructure.ApiResponse<Airbnb.PropertyService.Features.GetNewPropertiesCount.NewPropertiesCountResponse>))]

// --- Admin: GetPropertiesByIds (UC-E2 Reports) ---
[JsonSerializable(typeof(Airbnb.PropertyService.Features.GetPropertiesByIds.Request), TypeInfoPropertyName = "AdminGetPropertiesByIdsRequest")]
[JsonSerializable(typeof(Airbnb.PropertyService.Features.GetPropertiesByIds.PropertyBasicInfo))]
[JsonSerializable(typeof(Airbnb.ServiceDefaults.Infrastructure.ApiResponse<List<Airbnb.PropertyService.Features.GetPropertiesByIds.PropertyBasicInfo>>))]
[JsonSerializable(typeof(Guid[]), TypeInfoPropertyName = "GuidArray")]

internal partial class PropertyJsonContext : JsonSerializerContext { }

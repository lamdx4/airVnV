using System.Text.Json.Serialization;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Infrastructure;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
// --- Login ---
[JsonSerializable(typeof(ApiResponse<Features.Login.Login.Response>), TypeInfoPropertyName = "LoginApiResponse")]
[JsonSerializable(typeof(Features.Login.Login.Response), TypeInfoPropertyName = "LoginResponseData")]
[JsonSerializable(typeof(Features.Login.Login.Request), TypeInfoPropertyName = "LoginRequest")]

// --- Registration ---
[JsonSerializable(typeof(ApiResponse<Features.RegisterUser.Register.Response>), TypeInfoPropertyName = "RegisterApiResponse")]
[JsonSerializable(typeof(Features.RegisterUser.Register.Response), TypeInfoPropertyName = "RegisterResponseData")]
[JsonSerializable(typeof(Features.RegisterUser.Register.Request), TypeInfoPropertyName = "RegisterRequest")]
[JsonSerializable(typeof(ApiResponse<Features.RegisterUser.Verify.Response>), TypeInfoPropertyName = "VerifyApiResponse")]
[JsonSerializable(typeof(Features.RegisterUser.Verify.Response), TypeInfoPropertyName = "VerifyResponseData")]
[JsonSerializable(typeof(Features.RegisterUser.Verify.Request), TypeInfoPropertyName = "VerifyRequest")]

// --- Auth Utils ---
[JsonSerializable(typeof(ApiResponse<Features.RefreshToken.Execute.Response>), TypeInfoPropertyName = "RefreshTokenApiResponse")]
[JsonSerializable(typeof(Features.RefreshToken.Execute.Response), TypeInfoPropertyName = "RefreshTokenResponseData")]
[JsonSerializable(typeof(Features.RefreshToken.Execute.Request), TypeInfoPropertyName = "RefreshTokenRequest")]
[JsonSerializable(typeof(ApiResponse<Features.GoogleAuth.Execute.Response>), TypeInfoPropertyName = "GoogleAuthApiResponse")]
[JsonSerializable(typeof(Features.GoogleAuth.Execute.Response), TypeInfoPropertyName = "GoogleAuthResponseData")]
[JsonSerializable(typeof(Features.GoogleAuth.Execute.Request), TypeInfoPropertyName = "GoogleAuthRequest")]

// --- Profile ---
[JsonSerializable(typeof(ApiResponse<Features.Profile.Get.Response>), TypeInfoPropertyName = "GetProfileApiResponse")]
[JsonSerializable(typeof(Features.Profile.Get.Response), TypeInfoPropertyName = "GetProfileResponseData")]
[JsonSerializable(typeof(ApiResponse<Features.Profile.Update.Response>), TypeInfoPropertyName = "UpdateProfileApiResponse")]
[JsonSerializable(typeof(Features.Profile.Update.Response), TypeInfoPropertyName = "UpdateProfileResponseData")]
[JsonSerializable(typeof(Features.Profile.Update.Request), TypeInfoPropertyName = "UpdateProfileRequest")]

// --- Shared ---
[JsonSerializable(typeof(FastEndpoints.ErrorResponse))]
internal partial class UserJsonContext : JsonSerializerContext { }

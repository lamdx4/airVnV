using System.Text.Json.Serialization;

namespace Airbnb.UserService.Infrastructure;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
// --- Login ---
[JsonSerializable(typeof(Features.Login.Login.Request), TypeInfoPropertyName = "LoginRequest")]
[JsonSerializable(typeof(Features.Login.Login.Response), TypeInfoPropertyName = "LoginResponse")]

// --- Registration ---
[JsonSerializable(typeof(Features.RegisterUser.Register.Request), TypeInfoPropertyName = "RegisterRequest")]
[JsonSerializable(typeof(Features.RegisterUser.Register.Response), TypeInfoPropertyName = "RegisterResponse")]
[JsonSerializable(typeof(Features.RegisterUser.Verify.Request), TypeInfoPropertyName = "VerifyRequest")]
[JsonSerializable(typeof(Features.RegisterUser.Verify.Response), TypeInfoPropertyName = "VerifyResponse")]

// --- Auth Utils ---
[JsonSerializable(typeof(Features.RefreshToken.Execute.Request), TypeInfoPropertyName = "RefreshTokenRequest")]
[JsonSerializable(typeof(Features.RefreshToken.Execute.Response), TypeInfoPropertyName = "RefreshTokenResponse")]
[JsonSerializable(typeof(Features.GoogleAuth.Execute.Request), TypeInfoPropertyName = "GoogleAuthRequest")]
[JsonSerializable(typeof(Features.GoogleAuth.Execute.Response), TypeInfoPropertyName = "GoogleAuthResponse")]

// --- Profile ---
[JsonSerializable(typeof(Features.Profile.Get.Response), TypeInfoPropertyName = "GetProfileResponse")]
[JsonSerializable(typeof(Features.Profile.Update.Request), TypeInfoPropertyName = "UpdateProfileRequest")]
[JsonSerializable(typeof(Features.Profile.Update.Response), TypeInfoPropertyName = "UpdateProfileResponse")]

// --- Shared ---
[JsonSerializable(typeof(FastEndpoints.ErrorResponse))]
internal partial class UserJsonContext : JsonSerializerContext { }

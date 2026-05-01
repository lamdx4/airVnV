using System.Text.Json.Serialization;
using Airbnb.UserService.Features.RegisterUser;

namespace Airbnb.UserService.Infrastructure;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(Request), TypeInfoPropertyName = "RegisterRequest")]
[JsonSerializable(typeof(Response), TypeInfoPropertyName = "RegisterResponse")]
[JsonSerializable(typeof(Features.Login.Request), TypeInfoPropertyName = "LoginRequest")]
[JsonSerializable(typeof(Features.Login.Response), TypeInfoPropertyName = "LoginResponse")]
[JsonSerializable(typeof(Features.GoogleAuth.Request), TypeInfoPropertyName = "GoogleAuthRequest")]
[JsonSerializable(typeof(Features.GoogleAuth.Response), TypeInfoPropertyName = "GoogleAuthResponse")]
[JsonSerializable(typeof(Features.RefreshToken.Request), TypeInfoPropertyName = "RefreshTokenRequest")]
[JsonSerializable(typeof(Features.RefreshToken.Response), TypeInfoPropertyName = "RefreshTokenResponse")]
[JsonSerializable(typeof(Features.Profile.ProfileResponse), TypeInfoPropertyName = "ProfileResponse")]
[JsonSerializable(typeof(Features.Profile.UpdateRequest), TypeInfoPropertyName = "ProfileUpdateRequest")]
[JsonSerializable(typeof(FastEndpoints.ErrorResponse))]
internal partial class UserJsonContext : JsonSerializerContext { }

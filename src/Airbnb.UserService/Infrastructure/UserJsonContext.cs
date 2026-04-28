using System.Text.Json.Serialization;
using Airbnb.UserService.Features.RegisterUser;

namespace Airbnb.UserService.Infrastructure;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(Request))]
[JsonSerializable(typeof(Response))]
[JsonSerializable(typeof(Features.Login.Request))]
[JsonSerializable(typeof(Features.Login.Response))]
[JsonSerializable(typeof(FastEndpoints.ErrorResponse))]
internal partial class UserJsonContext : JsonSerializerContext { }

using System.Text.Json.Serialization;
using Airbnb.PropertyService.Features.CreateProperty;

namespace Airbnb.PropertyService.Infrastructure;

[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
[JsonSerializable(typeof(Request))]
[JsonSerializable(typeof(Response))]
[JsonSerializable(typeof(FastEndpoints.ErrorResponse))] // Quan trọng cho validation errors
internal partial class PropertyJsonContext : JsonSerializerContext { }

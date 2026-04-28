using System.Text.Json.Serialization;
using Airbnb.SearchService.Features.SearchProperties;
using Airbnb.SearchService.Domain;

namespace Airbnb.SearchService.Infrastructure;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(Request))]
[JsonSerializable(typeof(Response))]
[JsonSerializable(typeof(PropertyDoc))]
[JsonSerializable(typeof(List<PropertyDoc>))]
[JsonSerializable(typeof(FastEndpoints.ErrorResponse))]
internal partial class SearchJsonContext : JsonSerializerContext { }

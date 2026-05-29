using System.Text.Json.Serialization;
using Airbnb.SearchService.Features.SearchProperties;
using Airbnb.SearchService.Domain;

namespace Airbnb.SearchService.Infrastructure;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(Request))]
[JsonSerializable(typeof(PagedResponse<PropertyDoc>))]
[JsonSerializable(typeof(PropertyDoc))]
[JsonSerializable(typeof(List<PropertyDoc>))]
[JsonSerializable(typeof(Airbnb.ServiceDefaults.Infrastructure.ApiResponse<PagedResponse<PropertyDoc>>))]
[JsonSerializable(typeof(FastEndpoints.ErrorResponse))]
internal partial class SearchJsonContext : JsonSerializerContext { }

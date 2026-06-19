using System.Text.Json.Serialization;

namespace Airbnb.ChatService.Features.WebRTC.GetCredentials;

public sealed record IceServer(
    [property: JsonPropertyName("urls")] string Urls,
    [property: JsonPropertyName("username")] string? Username = null,
    [property: JsonPropertyName("credential")] string? Credential = null
);

public sealed record Response(
    List<IceServer> IceServers
);

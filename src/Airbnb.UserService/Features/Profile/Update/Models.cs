using FastEndpoints;
using Airbnb.UserService.Domain;
using System.Text.Json.Serialization;

namespace Airbnb.UserService.Features.Profile.Update;

public record Request(
    string FullName,
    string? AvatarUrl,
    string? PhoneNumber,
    string? Bio
) : ICommand<Response>
{
    [JsonIgnore]
    public Guid UserId { get; set; }
}

public record Response(
    Guid UserId,
    string Email,
    string FullName,
    string? AvatarUrl,
    string? PhoneNumber,
    string? Bio,
    UserRole Role
);

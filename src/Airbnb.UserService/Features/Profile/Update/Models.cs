using FastEndpoints;
using Airbnb.UserService.Domain;
using System.Text.Json.Serialization;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Profile.Update;

public record Request(
    string FullName,
    string? AvatarUrl,
    string? PhoneNumber,
    string? Bio
) : ICommand<ApiResponse<Response>>
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

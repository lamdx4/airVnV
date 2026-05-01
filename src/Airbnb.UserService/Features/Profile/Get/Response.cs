using Airbnb.UserService.Domain;

namespace Airbnb.UserService.Features.Profile.Get;

public record Response(
    Guid UserId,
    string Email,
    string FullName,
    string? AvatarUrl,
    string? PhoneNumber,
    string? Bio,
    UserRole Role
);

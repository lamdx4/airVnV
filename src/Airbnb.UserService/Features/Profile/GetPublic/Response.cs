namespace Airbnb.UserService.Features.Profile.GetPublic;

public record Response(
    Guid Id,
    string FullName,
    string? AvatarUrl
);

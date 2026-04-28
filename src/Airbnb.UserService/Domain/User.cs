namespace Airbnb.UserService.Domain;

public enum UserRole
{
    Guest,
    Host
}

public class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = default!;
    public string HashedPassword { get; private set; } = default!;
    public UserRole Role { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // 1:1 Mapping
    public UserProfile Profile { get; private set; } = default!;

    private User() { }

    public User(string email, string hashedPassword, UserRole role, string fullName)
    {
        Id = Guid.NewGuid();
        Email = email;
        HashedPassword = hashedPassword;
        Role = role;
        CreatedAt = DateTime.UtcNow;
        Profile = new UserProfile(Id, fullName);
    }
}

public class UserProfile
{
    public Guid UserId { get; private set; }
    public string FullName { get; private set; } = default!;
    public string? AvatarUrl { get; private set; }
    public string? PhoneNumber { get; private set; }
    public string? Bio { get; private set; }

    private UserProfile() { }

    public UserProfile(Guid userId, string fullName)
    {
        UserId = userId;
        FullName = fullName;
    }
}

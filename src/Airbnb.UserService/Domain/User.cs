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
    public string? HashedPassword { get; private set; } // Nullable cho Social Login
    public UserRole Role { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Relationships
    public UserProfile Profile { get; private set; } = default!;
    public ICollection<UserLogin> Logins { get; private set; } = new List<UserLogin>();

    private User() { }

    public User(string email, string? hashedPassword, UserRole role, string fullName)
    {
        Id = Guid.NewGuid();
        Email = email;
        HashedPassword = hashedPassword;
        Role = role;
        CreatedAt = DateTime.UtcNow;
        Profile = new UserProfile(Id, fullName);
    }

    public void AddLogin(string provider, string providerKey)
    {
        Logins.Add(new UserLogin(Id, provider, providerKey));
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

public class UserLogin
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Provider { get; private set; } = default!;
    public string ProviderKey { get; private set; } = default!;

    private UserLogin() { }

    public UserLogin(Guid userId, string provider, string providerKey)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Provider = provider;
        ProviderKey = providerKey;
    }
}

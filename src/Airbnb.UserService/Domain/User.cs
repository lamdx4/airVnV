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

    // 1. Khởi tạo cho Local User (Bắt buộc có Password)
    public User(string email, string hashedPassword, UserRole role, string fullName)
    {
        if (string.IsNullOrWhiteSpace(hashedPassword))
            throw new ArgumentException("Mật khẩu là bắt buộc đối với tài khoản cục bộ.");

        Id = Guid.NewGuid();
        Email = email;
        HashedPassword = hashedPassword;
        Role = role;
        CreatedAt = DateTime.UtcNow;
        Profile = new UserProfile(Id, fullName);
    }

    // 2. Khởi tạo cho Social User (Bắt buộc có Provider và Key)
    public User(string email, UserRole role, string fullName, string provider, string providerKey)
    {
        if (string.IsNullOrWhiteSpace(provider) || string.IsNullOrWhiteSpace(providerKey))
            throw new ArgumentException("Thông tin Provider là bắt buộc đối với tài khoản MXH.");

        Id = Guid.NewGuid();
        Email = email;
        HashedPassword = null;
        Role = role;
        CreatedAt = DateTime.UtcNow;
        Profile = new UserProfile(Id, fullName);
        AddLogin(provider, providerKey);
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

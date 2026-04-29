namespace Airbnb.UserService.Domain;

public enum UserRole
{
    Guest,
    Host
}

public enum AuthProvider
{
    Local,
    Google,
    Facebook
}

public class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = default!;
    public string? HashedPassword { get; private set; } 
    public UserRole Role { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Relationships
    public UserProfile Profile { get; private set; } = default!;
    public ICollection<UserLogin> Logins { get; private set; } = new List<UserLogin>();
    public ICollection<UserRefreshToken> RefreshTokens { get; private set; } = new List<UserRefreshToken>();

    private User() { }

    // 1. Khởi tạo cho Local User
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

    // 2. Khởi tạo cho Social User
    public User(string email, UserRole role, string fullName, AuthProvider provider, string providerKey)
    {
        if (string.IsNullOrWhiteSpace(providerKey))
            throw new ArgumentException("Thông tin Provider Key là bắt buộc.");

        Id = Guid.NewGuid();
        Email = email;
        HashedPassword = null;
        Role = role;
        CreatedAt = DateTime.UtcNow;
        Profile = new UserProfile(Id, fullName);
        AddLogin(provider, providerKey);
    }

    public void AddLogin(AuthProvider provider, string providerKey)
    {
        Logins.Add(new UserLogin(Id, provider, providerKey));
    }

    public void AddRefreshToken(string token, DateTime expiresAt)
    {
        RefreshTokens.Add(new UserRefreshToken(Id, token, expiresAt));
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
    public User User { get; private set; } = default!;
    public AuthProvider Provider { get; private set; }
    public string ProviderKey { get; private set; } = default!;

    private UserLogin() { }

    public UserLogin(Guid userId, AuthProvider provider, string providerKey)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Provider = provider;
        ProviderKey = providerKey;
    }
}

public class UserRefreshToken
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public User User { get; private set; } = default!;
    public string Token { get; private set; } = default!;
    public DateTime ExpiresAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime LoginAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsActive => RevokedAt == null && !IsExpired;

    private UserRefreshToken() { }

    public UserRefreshToken(Guid userId, string token, DateTime expiresAt)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Token = token;
        ExpiresAt = expiresAt;
        CreatedAt = DateTime.UtcNow;
        LoginAt = DateTime.UtcNow;
    }

    public void Revoke()
    {
        RevokedAt = DateTime.UtcNow;
    }
}

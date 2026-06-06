using System.Text.Json.Serialization;

using Airbnb.ServiceDefaults.Infrastructure;
using Airbnb.SharedKernel.Domain;
using Airbnb.UserService.Domain.Events;

namespace Airbnb.UserService.Domain;

[JsonConverter(typeof(JsonStringEnumConverter<UserRole>))]
public enum UserRole
{
    User,
    Moderator,
    Admin
}

[JsonConverter(typeof(JsonStringEnumConverter<UserStatus>))]
public enum UserStatus
{
    Active,
    Suspended,
    Banned
}

public enum AuthProvider
{
    Local,
    Google,
    Facebook
}

public class User : AggregateRoot
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = default!;
    public string? HashedPassword { get; private set; } 
    public UserRole Role { get; private set; }
    public UserStatus Status { get; private set; }
    public bool IsVerified { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    public string? SuspensionReason { get; private set; }
    public string? BanReason { get; private set; }

    // Relationships
    public UserProfile Profile { get; private set; } = default!;
    public ICollection<UserLogin> Logins { get; private set; } = new List<UserLogin>();
    public ICollection<UserRefreshToken> RefreshTokens { get; private set; } = new List<UserRefreshToken>();

    public void SetPassword(string password)
    {
        HashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
    }

    private User() { }

    // 1. Khởi tạo cho Local User
    public User(string email, string password, UserRole role, string fullName)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new BusinessException("Mật khẩu là bắt buộc đối với tài khoản cục bộ.", "USER_PASSWORD_REQUIRED");

        Id = Guid.CreateVersion7();
        Email = email;
        HashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
        Role = role;
        Status = UserStatus.Active;
        IsVerified = false;
        CreatedAt = DateTime.UtcNow;
        Profile = new UserProfile(Id, fullName);
    }

    // 2. Khởi tạo cho Social User
    public User(string email, UserRole role, string fullName, AuthProvider provider, string providerKey)
    {
        if (string.IsNullOrWhiteSpace(providerKey))
            throw new BusinessException("Thông tin Provider Key là bắt buộc.", "USER_PROVIDER_KEY_REQUIRED");

        Id = Guid.CreateVersion7();
        Email = email;
        HashedPassword = null;
        Role = role;
        Status = UserStatus.Active;
        IsVerified = false;
        CreatedAt = DateTime.UtcNow;
        Profile = new UserProfile(Id, fullName);
        AddLogin(provider, providerKey);
    }

    public void AddLogin(AuthProvider provider, string providerKey)
    {
        Logins.Add(new UserLogin(Id, provider, providerKey));
    }

    public void AddRefreshToken(string token, DateTime expiresAt, string? userAgent = null, string? ipAddress = null)
    {
        RefreshTokens.Add(new UserRefreshToken(Id, token, expiresAt, userAgent, ipAddress));
    }

    public void UpdateProfile(string fullName, string? avatarUrl, string? phoneNumber, string? bio)
    {
        Profile.UpdateInfo(fullName, avatarUrl, phoneNumber, bio);
        Raise(new UserProfileUpdatedDomainEvent(Id, fullName, avatarUrl));
    }

    public void Suspend(string reason)
    {
        if (Status is not UserStatus.Active)
            throw new BusinessException("Only active users can be suspended.", "INVALID_STATUS_TRANSITION");
        if (string.IsNullOrWhiteSpace(reason))
            throw new BusinessException("Suspension reason is required.", "REASON_REQUIRED");
        Status = UserStatus.Suspended;
        SuspensionReason = reason;
    }

    public void Ban(string reason)
    {
        if (Status is UserStatus.Banned)
            throw new BusinessException("User is already banned.", "USER_ALREADY_BANNED");
        if (Status is not (UserStatus.Active or UserStatus.Suspended))
            throw new BusinessException("Cannot ban a user with current status.", "INVALID_STATUS_TRANSITION");
        if (string.IsNullOrWhiteSpace(reason))
            throw new BusinessException("Ban reason is required.", "REASON_REQUIRED");
        Status = UserStatus.Banned;
        BanReason = reason;
    }

    public void Activate()
    {
        if (Status is not (UserStatus.Suspended or UserStatus.Banned))
            throw new BusinessException("Only suspended or banned users can be activated.", "INVALID_STATUS_TRANSITION");
        Status = UserStatus.Active;
        SuspensionReason = null;
        BanReason = null;
    }

    public void SetLastLoginAt(DateTime lastLoginAt)
    {
        LastLoginAt = lastLoginAt;
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

    public void UpdateInfo(string fullName, string? avatarUrl, string? phoneNumber, string? bio)
    {
        FullName = fullName;
        AvatarUrl = avatarUrl;
        PhoneNumber = phoneNumber;
        Bio = bio;
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
        Id = Guid.CreateVersion7();
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
    public string? UserAgent { get; private set; }
    public string? IpAddress { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime LoginAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsActive => RevokedAt == null && !IsExpired;

    private UserRefreshToken() { }

    public UserRefreshToken(Guid userId, string token, DateTime expiresAt, string? userAgent = null, string? ipAddress = null)
    {
        Id = Guid.CreateVersion7();
        UserId = userId;
        Token = token;
        UserAgent = userAgent;
        IpAddress = ipAddress;
        ExpiresAt = expiresAt;
        CreatedAt = DateTime.UtcNow;
        LoginAt = DateTime.UtcNow;
    }

    public void Revoke()
    {
        RevokedAt = DateTime.UtcNow;
    }
}



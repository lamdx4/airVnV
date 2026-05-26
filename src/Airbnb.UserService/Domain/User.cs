using System.Text.Json.Serialization;

using Airbnb.SharedKernel.Domain;
using Airbnb.UserService.Domain.Events;
using Airbnb.ServiceDefaults.Infrastructure;

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
    Banned,
    PendingVerification
}

[JsonConverter(typeof(JsonStringEnumConverter<KycStatus>))]
public enum KycStatus
{
    NotSubmitted,
    Pending,
    Approved,
    Rejected
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
    public UserStatus Status { get; private set; } = UserStatus.Active;
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // KYC
    public KycStatus KycStatus { get; private set; } = KycStatus.NotSubmitted;
    public DateTime? KycSubmittedAt { get; private set; }
    public DateTime? KycVerifiedAt { get; private set; }
    public string? KycRejectionReason { get; private set; }

    // Relationships
    public UserProfile Profile { get; private set; } = default!;
    public ICollection<UserLogin> Logins { get; private set; } = new List<UserLogin>();
    public ICollection<UserRefreshToken> RefreshTokens { get; private set; } = new List<UserRefreshToken>();
    public ICollection<UserSuspension> Suspensions { get; private set; } = new List<UserSuspension>();

    public bool IsVerified => KycStatus == KycStatus.Approved;

    public void SetPassword(string hashedPassword)
    {
        HashedPassword = hashedPassword;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Suspend(string reason, int? durationDays = null)
    {
        if (Status == UserStatus.Banned)
            throw new BusinessException("Banned users cannot be suspended.", "USER_BANNED");

        var suspension = new UserSuspension(Id, reason, durationDays);
        Suspensions.Add(suspension);
        Status = UserStatus.Suspended;
    }

    public void Unsuspend()
    {
        if (Status != UserStatus.Suspended)
            throw new BusinessException("Only suspended users can be unsuspended.", "USER_NOT_SUSPENDED");

        Status = UserStatus.Active;
        
        // Clear active suspensions
        foreach (var suspension in Suspensions.Where(s => s.IsActive))
        {
            suspension.Revoke();
        }
    }

    public void SubmitKyc()
    {
        if (KycStatus == KycStatus.Approved)
            throw new BusinessException("User already verified.", "USER_ALREADY_VERIFIED");

        KycStatus = KycStatus.Pending;
        KycSubmittedAt = DateTime.UtcNow;
    }

    public void ApproveKyc()
    {
        if (KycStatus != KycStatus.Pending)
            throw new BusinessException("Only pending KYC can be approved.", "KYC_NOT_PENDING");

        KycStatus = KycStatus.Approved;
        KycVerifiedAt = DateTime.UtcNow;
    }

    public void RejectKyc(string reason)
    {
        if (KycStatus != KycStatus.Pending)
            throw new BusinessException("Only pending KYC can be rejected.", "KYC_NOT_PENDING");

        KycStatus = KycStatus.Rejected;
        KycVerifiedAt = DateTime.UtcNow;
        KycRejectionReason = reason;
    }

    private User() { }

    // 1. Khởi tạo cho Local User
    public User(string email, string hashedPassword, UserRole role, string fullName)
    {
        if (string.IsNullOrWhiteSpace(hashedPassword))
            throw new ArgumentException("Mật khẩu là bắt buộc đối với tài khoản cục bộ.");

        Id = Guid.CreateVersion7();
        Email = email;
        HashedPassword = hashedPassword;
        Role = role;
        Status = UserStatus.Active;
        CreatedAt = DateTime.UtcNow;
        Profile = new UserProfile(Id, fullName);
    }

    // 2. Khởi tạo cho Social User
    public User(string email, UserRole role, string fullName, AuthProvider provider, string providerKey)
    {
        if (string.IsNullOrWhiteSpace(providerKey))
            throw new ArgumentException("Thông tin Provider Key là bắt buộc.");

        Id = Guid.CreateVersion7();
        Email = email;
        HashedPassword = null;
        Role = role;
        Status = UserStatus.Active;
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

public class UserSuspension
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public User User { get; private set; } = default!;
    public string Reason { get; private set; } = default!;
    public DateTime SuspendedAt { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }

    public bool IsActive => RevokedAt == null && (ExpiresAt == null || ExpiresAt > DateTime.UtcNow);

    private UserSuspension() { }

    public UserSuspension(Guid userId, string reason, int? durationDays = null)
    {
        Id = Guid.CreateVersion7();
        UserId = userId;
        Reason = reason;
        SuspendedAt = DateTime.UtcNow;
        ExpiresAt = durationDays.HasValue ? DateTime.UtcNow.AddDays(durationDays.Value) : null;
    }

    public void Revoke()
    {
        RevokedAt = DateTime.UtcNow;
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
    public DateTime? UpdatedAt { get; private set; }
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
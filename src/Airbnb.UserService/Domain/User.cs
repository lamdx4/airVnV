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
    Banned,
    PendingVerification
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
    public ICollection<KycDocument> KycDocuments { get; private set; } = new List<KycDocument>();

    public void SetPassword(string hashedPassword)
    {
        HashedPassword = hashedPassword;
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
        IsVerified = false;
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
        if (Status is not (UserStatus.Active or UserStatus.PendingVerification))
            throw new BusinessException("Only active or pending-verification users can be suspended.", "INVALID_STATUS_TRANSITION");
        if (string.IsNullOrWhiteSpace(reason))
            throw new BusinessException("Suspension reason is required.", "REASON_REQUIRED");
        Status = UserStatus.Suspended;
        SuspensionReason = reason;
    }

    public void Ban(string reason)
    {
        if (Status is UserStatus.Banned)
            throw new BusinessException("User is already banned.", "USER_ALREADY_BANNED");
        if (Status is not (UserStatus.Active or UserStatus.Suspended or UserStatus.PendingVerification))
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

    public void ApproveVerification()
    {
        if (Role is not UserRole.User)
            throw new BusinessException("KYC verification is only applicable to host users.", "INVALID_ROLE_FOR_KYC");
        IsVerified = true;
        Status = UserStatus.Active;
    }

    public void RejectVerification(string reason)
    {
        if (Role is not UserRole.User)
            throw new BusinessException("KYC verification is only applicable to host users.", "INVALID_ROLE_FOR_KYC");
        if (string.IsNullOrWhiteSpace(reason))
            throw new BusinessException("Rejection reason is required.", "REASON_REQUIRED");
        IsVerified = false;
    }

    public void SetLastLoginAt(DateTime lastLoginAt)
    {
        LastLoginAt = lastLoginAt;
    }

    public void SetPendingVerification()
    {
        Status = UserStatus.PendingVerification;
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

[JsonConverter(typeof(JsonStringEnumConverter<KycDocumentStatus>))]
public enum KycDocumentStatus
{
    Submitted,
    Approved,
    Rejected,
    Expired
}

public class KycDocument
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public User User { get; private set; } = default!;
    public KycDocumentStatus Status { get; private set; }
    public string? DocumentType { get; private set; }
    public string? RejectionReason { get; private set; }
    public DateTime SubmittedAt { get; private set; }
    public DateTime? ReviewedAt { get; private set; }
    public ICollection<KycDocumentImage> Images { get; private set; } = new List<KycDocumentImage>();

    private KycDocument() { }

    public KycDocument(Guid userId, string? documentType)
    {
        Id = Guid.CreateVersion7();
        UserId = userId;
        DocumentType = documentType;
        Status = KycDocumentStatus.Submitted;
        SubmittedAt = DateTime.UtcNow;
    }

    public void Approve()
    {
        Status = KycDocumentStatus.Approved;
        ReviewedAt = DateTime.UtcNow;
    }

    public void Reject(string reason)
    {
        Status = KycDocumentStatus.Rejected;
        RejectionReason = reason;
        ReviewedAt = DateTime.UtcNow;
    }

    public void AddImage(string imageUrl, string? label = null)
    {
        Images.Add(new KycDocumentImage(Id, imageUrl, label));
    }
}

public class KycDocumentImage
{
    public Guid Id { get; private set; }
    public Guid KycDocumentId { get; private set; }
    public KycDocument KycDocument { get; private set; } = default!;
    public string ImageUrl { get; private set; } = default!;
    public string? Label { get; private set; }

    private KycDocumentImage() { }

    public KycDocumentImage(Guid kycDocumentId, string imageUrl, string? label = null)
    {
        Id = Guid.CreateVersion7();
        KycDocumentId = kycDocumentId;
        ImageUrl = imageUrl;
        Label = label;
    }
}

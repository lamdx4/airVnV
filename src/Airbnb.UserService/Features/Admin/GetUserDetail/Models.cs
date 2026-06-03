using FastEndpoints;
using Airbnb.UserService.Domain;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Admin.GetUserDetail;

public record GetUserDetailRequest(Guid Id) : Mediator.IQuery<ApiResponse<UserDetailResponse>?>;

public record UserDetailResponse(
    Guid Id,
    string Email,
    string FullName,
    string? AvatarUrl,
    string? PhoneNumber,
    string? Bio,
    UserRole Role,
    UserStatus Status,
    bool IsVerified,
    DateTime CreatedAt,
    DateTime? LastLoginAt,
    string? SuspensionReason,
    string? BanReason,
    List<KycDocumentSummary>? KycDocuments
);

public record KycDocumentSummary(
    Guid Id,
    string Status,
    string? DocumentType,
    string? RejectionReason,
    DateTime SubmittedAt,
    DateTime? ReviewedAt,
    List<KycImageSummary> Images
);

public record KycImageSummary(
    Guid Id,
    string ImageUrl,
    string? Label
);

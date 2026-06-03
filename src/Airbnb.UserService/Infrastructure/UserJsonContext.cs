using System.Text.Json.Serialization;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Infrastructure;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
// --- Login ---
[JsonSerializable(typeof(ApiResponse<Features.Login.Login.Response>), TypeInfoPropertyName = "LoginApiResponse")]
[JsonSerializable(typeof(Features.Login.Login.Response), TypeInfoPropertyName = "LoginResponseData")]
[JsonSerializable(typeof(Features.Login.Login.Request), TypeInfoPropertyName = "LoginRequest")]

// --- Registration ---
[JsonSerializable(typeof(ApiResponse<Features.RegisterUser.Register.Response>), TypeInfoPropertyName = "RegisterApiResponse")]
[JsonSerializable(typeof(Features.RegisterUser.Register.Response), TypeInfoPropertyName = "RegisterResponseData")]
[JsonSerializable(typeof(Features.RegisterUser.Register.Request), TypeInfoPropertyName = "RegisterRequest")]
[JsonSerializable(typeof(ApiResponse<Features.RegisterUser.Verify.Response>), TypeInfoPropertyName = "VerifyApiResponse")]
[JsonSerializable(typeof(Features.RegisterUser.Verify.Response), TypeInfoPropertyName = "VerifyResponseData")]
[JsonSerializable(typeof(Features.RegisterUser.Verify.Request), TypeInfoPropertyName = "VerifyRequest")]

// --- Auth Utils ---
[JsonSerializable(typeof(ApiResponse<Features.RefreshToken.Execute.Response>), TypeInfoPropertyName = "RefreshTokenApiResponse")]
[JsonSerializable(typeof(Features.RefreshToken.Execute.Response), TypeInfoPropertyName = "RefreshTokenResponseData")]
[JsonSerializable(typeof(Features.RefreshToken.Execute.Request), TypeInfoPropertyName = "RefreshTokenRequest")]
[JsonSerializable(typeof(ApiResponse<Features.GoogleAuth.Execute.Response>), TypeInfoPropertyName = "GoogleAuthApiResponse")]
[JsonSerializable(typeof(Features.GoogleAuth.Execute.Response), TypeInfoPropertyName = "GoogleAuthResponseData")]
[JsonSerializable(typeof(Features.GoogleAuth.Execute.Request), TypeInfoPropertyName = "GoogleAuthRequest")]

// --- Profile ---
[JsonSerializable(typeof(ApiResponse<Features.Profile.Get.Response>), TypeInfoPropertyName = "GetProfileApiResponse")]
[JsonSerializable(typeof(Features.Profile.Get.Response), TypeInfoPropertyName = "GetProfileResponseData")]
[JsonSerializable(typeof(ApiResponse<Features.Profile.Update.Response>), TypeInfoPropertyName = "UpdateProfileApiResponse")]
[JsonSerializable(typeof(Features.Profile.Update.Response), TypeInfoPropertyName = "UpdateProfileResponseData")]
[JsonSerializable(typeof(Features.Profile.Update.Request), TypeInfoPropertyName = "UpdateProfileRequest")]

// --- Media ---
[JsonSerializable(typeof(ApiResponse<Airbnb.Infrastructure.Media.SignatureResponse>), TypeInfoPropertyName = "GetSignatureApiResponse")]
[JsonSerializable(typeof(Features.Media.GetSignature.Request), TypeInfoPropertyName = "GetSignatureRequest")]
[JsonSerializable(typeof(Airbnb.Infrastructure.Media.SignatureResponse))]

// --- Admin: GetUsers ---
[JsonSerializable(typeof(ApiResponse<Features.Admin.GetUsers.PaginatedUserListResponse>), TypeInfoPropertyName = "AdminGetUsersApiResponse")]
[JsonSerializable(typeof(Features.Admin.GetUsers.PaginatedUserListResponse), TypeInfoPropertyName = "AdminGetUsersResponseData")]
[JsonSerializable(typeof(Features.Admin.GetUsers.UserSummaryResponse), TypeInfoPropertyName = "AdminUserSummaryResponse")]
[JsonSerializable(typeof(Features.Admin.GetUsers.Request), TypeInfoPropertyName = "AdminGetUsersRequest")]

// --- Admin: GetUserDetail ---
[JsonSerializable(typeof(ApiResponse<Features.Admin.GetUserDetail.UserDetailResponse>), TypeInfoPropertyName = "AdminGetUserDetailApiResponse")]
[JsonSerializable(typeof(Features.Admin.GetUserDetail.UserDetailResponse), TypeInfoPropertyName = "AdminUserDetailResponseData")]
[JsonSerializable(typeof(Features.Admin.GetUserDetail.KycDocumentSummary), TypeInfoPropertyName = "AdminKycDocumentSummary")]
[JsonSerializable(typeof(Features.Admin.GetUserDetail.KycImageSummary), TypeInfoPropertyName = "AdminKycImageSummary")]

// --- Admin: SuspendUser ---
[JsonSerializable(typeof(ApiResponse<Features.Admin.SuspendUser.UserActionResponse>), TypeInfoPropertyName = "AdminSuspendUserApiResponse")]
[JsonSerializable(typeof(Features.Admin.SuspendUser.UserActionResponse), TypeInfoPropertyName = "AdminSuspendUserResponseData")]
[JsonSerializable(typeof(Features.Admin.SuspendUser.Request), TypeInfoPropertyName = "AdminSuspendUserRequest")]

// --- Admin: BanUser ---
[JsonSerializable(typeof(ApiResponse<Features.Admin.BanUser.BanUserResponse>), TypeInfoPropertyName = "AdminBanUserApiResponse")]
[JsonSerializable(typeof(Features.Admin.BanUser.BanUserResponse), TypeInfoPropertyName = "AdminBanUserResponseData")]
[JsonSerializable(typeof(Features.Admin.BanUser.Request), TypeInfoPropertyName = "AdminBanUserRequest")]

// --- Admin: ActivateUser ---
[JsonSerializable(typeof(ApiResponse<Features.Admin.ActivateUser.ActivateUserResponse>), TypeInfoPropertyName = "AdminActivateUserApiResponse")]
[JsonSerializable(typeof(Features.Admin.ActivateUser.ActivateUserResponse), TypeInfoPropertyName = "AdminActivateUserResponseData")]

// --- Admin: GetKycDocuments ---
[JsonSerializable(typeof(ApiResponse<List<Features.Admin.GetKycDocuments.KycDocumentDetailResponse>>), TypeInfoPropertyName = "AdminGetKycDocumentsApiResponse")]
[JsonSerializable(typeof(Features.Admin.GetKycDocuments.KycDocumentDetailResponse), TypeInfoPropertyName = "AdminKycDocumentDetailResponse")]
[JsonSerializable(typeof(Features.Admin.GetKycDocuments.KycImageDetailResponse), TypeInfoPropertyName = "AdminKycImageDetailResponse")]

// --- Admin: ApproveVerification ---
[JsonSerializable(typeof(ApiResponse<Features.Admin.ApproveVerification.VerificationResponse>), TypeInfoPropertyName = "AdminApproveVerificationApiResponse")]
[JsonSerializable(typeof(Features.Admin.ApproveVerification.VerificationResponse), TypeInfoPropertyName = "AdminApproveVerificationResponseData")]

// --- Admin: RejectVerification ---
[JsonSerializable(typeof(ApiResponse<Features.Admin.RejectVerification.RejectVerificationResponse>), TypeInfoPropertyName = "AdminRejectVerificationApiResponse")]
[JsonSerializable(typeof(Features.Admin.RejectVerification.RejectVerificationResponse), TypeInfoPropertyName = "AdminRejectVerificationResponseData")]
[JsonSerializable(typeof(Features.Admin.RejectVerification.RejectVerificationRequest), TypeInfoPropertyName = "AdminRejectVerificationRequest")]

// --- Shared ---
[JsonSerializable(typeof(FastEndpoints.ErrorResponse))]
internal partial class UserJsonContext : JsonSerializerContext { }

using FastEndpoints;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Admin.ApproveVerification;

public record ApproveVerificationRequest(Guid Id) : ICommand<ApiResponse<VerificationResponse>>;

public record VerificationResponse(Guid Id, bool IsVerified, string Status, string Message);

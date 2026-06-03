using FastEndpoints;
using FluentValidation;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Admin.RejectVerification;

public record RejectVerificationRequest(Guid Id, string Reason) : ICommand<ApiResponse<RejectVerificationResponse>>;

public record RejectVerificationResponse(Guid Id, bool IsVerified, string Message);

public class Validator : Validator<RejectVerificationRequest>
{
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(500);
    }
}

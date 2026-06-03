using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Airbnb.UserService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Admin.ApproveVerification;

public sealed class ApproveVerificationHandler(UserDbContext db)
    : ICommandHandler<ApproveVerificationRequest, ApiResponse<VerificationResponse>>
{
    public async Task<ApiResponse<VerificationResponse>> ExecuteAsync(ApproveVerificationRequest req, CancellationToken ct)
    {
        var user = await db.Users
            .Include(u => u.KycDocuments)
            .FirstOrDefaultAsync(u => u.Id == req.Id, ct);

        if (user is null)
            throw new NotFoundException("User not found.");

        user.ApproveVerification();

        var pendingDoc = user.KycDocuments
            .FirstOrDefault(d => d.Status == Domain.KycDocumentStatus.Submitted);
        pendingDoc?.Approve();

        await db.SaveChangesAsync(ct);

        return ApiResponse<VerificationResponse>.SuccessResult(
            new VerificationResponse(user.Id, user.IsVerified, user.Status.ToString(), "Identity verified successfully"),
            "Verification approved"
        );
    }
}

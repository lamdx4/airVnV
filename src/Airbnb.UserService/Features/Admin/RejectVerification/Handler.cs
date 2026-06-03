using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Airbnb.UserService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Admin.RejectVerification;

public sealed class RejectVerificationHandler(UserDbContext db)
    : ICommandHandler<RejectVerificationRequest, ApiResponse<RejectVerificationResponse>>
{
    public async Task<ApiResponse<RejectVerificationResponse>> ExecuteAsync(RejectVerificationRequest req, CancellationToken ct)
    {
        var user = await db.Users
            .Include(u => u.KycDocuments)
            .FirstOrDefaultAsync(u => u.Id == req.Id, ct);

        if (user is null)
            throw new NotFoundException("User not found.");

        user.RejectVerification(req.Reason);

        var pendingDoc = user.KycDocuments
            .FirstOrDefault(d => d.Status == Domain.KycDocumentStatus.Submitted);
        pendingDoc?.Reject(req.Reason);

        await db.SaveChangesAsync(ct);

        return ApiResponse<RejectVerificationResponse>.SuccessResult(
            new RejectVerificationResponse(user.Id, user.IsVerified, "Verification rejected"),
            "Verification rejected"
        );
    }
}

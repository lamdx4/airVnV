using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.UserService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Admin.GetUserDetail;

public sealed class GetUserDetailHandler(UserDbContext db)
    : IQueryHandler<GetUserDetailRequest, ApiResponse<UserDetailResponse>?>
{
    public async ValueTask<ApiResponse<UserDetailResponse>?> Handle(GetUserDetailRequest req, CancellationToken ct)
    {
        var user = await db.Users
            .AsNoTracking()
            .Include(u => u.Profile)
            .Include(u => u.KycDocuments)
                .ThenInclude(d => d.Images)
            .Where(u => u.Id == req.Id)
            .Select(u => new UserDetailResponse(
                u.Id,
                u.Email,
                u.Profile.FullName,
                u.Profile.AvatarUrl,
                u.Profile.PhoneNumber,
                u.Profile.Bio,
                u.Role,
                u.Status,
                u.IsVerified,
                u.CreatedAt,
                u.LastLoginAt,
                u.SuspensionReason,
                u.BanReason,
                u.KycDocuments.Select(d => new KycDocumentSummary(
                    d.Id,
                    d.Status.ToString(),
                    d.DocumentType,
                    d.RejectionReason,
                    d.SubmittedAt,
                    d.ReviewedAt,
                    d.Images.Select(i => new KycImageSummary(i.Id, i.ImageUrl, i.Label)).ToList()
                )).ToList()
            ))
            .FirstOrDefaultAsync(ct);

        if (user is null)
            return null;

        return ApiResponse<UserDetailResponse>.SuccessResult(user, "User detail retrieved");
    }
}

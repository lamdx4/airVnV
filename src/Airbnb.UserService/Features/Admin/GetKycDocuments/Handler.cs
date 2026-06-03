using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.UserService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Admin.GetKycDocuments;

public sealed class GetKycDocumentsHandler(UserDbContext db)
    : IQueryHandler<GetKycDocumentsRequest, ApiResponse<List<KycDocumentDetailResponse>>?>
{
    public async ValueTask<ApiResponse<List<KycDocumentDetailResponse>>?> Handle(GetKycDocumentsRequest req, CancellationToken ct)
    {
        var user = await db.Users
            .AsNoTracking()
            .Include(u => u.KycDocuments)
                .ThenInclude(d => d.Images)
            .Where(u => u.Id == req.Id)
            .Select(u => new
            {
                u.Id,
                u.Role,
                Documents = u.KycDocuments.Select(d => new KycDocumentDetailResponse(
                    d.Id,
                    d.Status,
                    d.DocumentType,
                    d.RejectionReason,
                    d.SubmittedAt,
                    d.ReviewedAt,
                    d.Images.Select(i => new KycImageDetailResponse(i.Id, i.ImageUrl, i.Label)).ToList()
                )).ToList()
            })
            .FirstOrDefaultAsync(ct);

        if (user is null)
            return null;

        return ApiResponse<List<KycDocumentDetailResponse>>.SuccessResult(
            user.Documents,
            "KYC documents retrieved"
        );
    }
}

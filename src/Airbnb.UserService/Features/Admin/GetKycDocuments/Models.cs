using FastEndpoints;
using Airbnb.UserService.Domain;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Admin.GetKycDocuments;

public record GetKycDocumentsRequest(Guid Id) : Mediator.IQuery<ApiResponse<List<KycDocumentDetailResponse>>?>;

public record KycDocumentDetailResponse(
    Guid Id,
    KycDocumentStatus Status,
    string? DocumentType,
    string? RejectionReason,
    DateTime SubmittedAt,
    DateTime? ReviewedAt,
    List<KycImageDetailResponse> Images
);

public record KycImageDetailResponse(
    Guid Id,
    string ImageUrl,
    string? Label
);

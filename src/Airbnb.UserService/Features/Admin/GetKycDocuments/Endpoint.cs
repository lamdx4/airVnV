using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Admin.GetKycDocuments;

public class Endpoint : EndpointWithoutRequest<ApiResponse<List<KycDocumentDetailResponse>>>
{
    private readonly IMediator _mediator;

    public Endpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/{Id}/kyc-documents");
        Group<AdminGroup>();
        Roles("Admin", "Moderator");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("Id");
        var result = await _mediator.Send(new GetKycDocumentsRequest(id), ct);

        if (result is null)
        {
            await Send.ResponseAsync(
                ApiResponse<List<KycDocumentDetailResponse>>.FailureResult("USER_NOT_FOUND", "User not found"), 404, ct);
            return;
        }

        Response = result;
    }
}

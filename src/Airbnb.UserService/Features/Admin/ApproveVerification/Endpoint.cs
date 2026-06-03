using FastEndpoints;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Admin.ApproveVerification;

public class Endpoint : EndpointWithoutRequest<ApiResponse<VerificationResponse>>
{
    public override void Configure()
    {
        Patch("/{Id}/verify");
        Group<AdminGroup>();
        Roles("Admin", "Moderator");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("Id");
        try
        {
            Response = await new ApproveVerificationRequest(id).ExecuteAsync(ct);
        }
        catch (BusinessException ex)
        {
            await Send.ResponseAsync(ApiResponse<VerificationResponse>.FailureResult(ex.ErrorCode, ex.Message), 400, ct);
        }
    }
}

using FastEndpoints;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Admin.RejectVerification;

public class Endpoint : Endpoint<RejectVerificationRequest, ApiResponse<RejectVerificationResponse>>
{
    public override void Configure()
    {
        Patch("/{Id}/reject-verification");
        Group<AdminGroup>();
        Roles("Admin", "Moderator");
    }

    public override async Task HandleAsync(RejectVerificationRequest req, CancellationToken ct)
    {
        var id = Route<Guid>("Id");
        req = req with { Id = id };
        try
        {
            Response = await req.ExecuteAsync(ct);
        }
        catch (BusinessException ex)
        {
            await Send.ResponseAsync(ApiResponse<RejectVerificationResponse>.FailureResult(ex.ErrorCode, ex.Message), 400, ct);
        }
    }
}

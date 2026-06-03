using FastEndpoints;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Admin.ActivateUser;

public class Endpoint : EndpointWithoutRequest<ApiResponse<ActivateUserResponse>>
{
    public override void Configure()
    {
        Patch("/{Id}/activate");
        Group<AdminGroup>();
        Roles("Admin", "Moderator");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("Id");
        try
        {
            Response = await new ActivateUserRequest(id).ExecuteAsync(ct);
        }
        catch (BusinessException ex)
        {
            await Send.ResponseAsync(ApiResponse<ActivateUserResponse>.FailureResult(ex.ErrorCode, ex.Message), 400, ct);
        }
    }
}

using FastEndpoints;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Admin.SuspendUser;

public class Endpoint : Endpoint<Request, ApiResponse<UserActionResponse>>
{
    public override void Configure()
    {
        Patch("/{Id}/suspend");
        Group<AdminGroup>();
        Roles("Admin", "Moderator");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var id = Route<Guid>("Id");
        req = req with { Id = id };
        try
        {
            Response = await req.ExecuteAsync(ct);
        }
        catch (BusinessException ex)
        {
            await Send.ResponseAsync(ApiResponse<UserActionResponse>.FailureResult(ex.ErrorCode, ex.Message), 400, ct);
        }
    }
}

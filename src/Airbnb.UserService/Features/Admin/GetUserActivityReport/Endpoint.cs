using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Admin.GetUserActivityReport;

public class Endpoint(IMediator mediator) : EndpointWithoutRequest<ApiResponse<Response>>
{
    public override void Configure()
    {
        Get("/user-activity");
        Group<ReportsGroup>();
        Roles("Admin", "Moderator");
        Summary(s =>
        {
            s.Summary = "Admin: user activity buckets by last login window";
            s.Description = "No error codes. Read-only aggregate.";
            s.Responses[200] = "User activity retrieved";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await mediator.Send(new Request(), ct);
        Response = ApiResponse<Response>.SuccessResult(result, "User activity retrieved");
    }
}

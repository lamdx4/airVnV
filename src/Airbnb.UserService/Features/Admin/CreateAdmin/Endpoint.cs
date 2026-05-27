using Mediator;
using Airbnb.UserService.Features.Admin.CreateAdmin;
using Airbnb.ServiceDefaults.Infrastructure;
using FastEndpoints;

namespace Airbnb.UserService.Features.Admin.CreateAdmin;

public class Endpoint(IMediator mediator) : FastEndpoints.Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Post("/api/admin/users/create-admin");
        AllowAnonymous(); // This endpoint creates the first admin
        Summary(s =>
        {
            s.Summary = "Create admin account (use for initial setup)";
            s.Description = "Creates a new admin user. Should be protected in production or used only for initial setup.";
            s.Responses[200] = "Admin account created successfully";
            s.Responses[400] = "Email already exists";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await Send.ResponseAsync(ApiResponse<Response>.SuccessResult(result, "Admin account created successfully"), cancellation: ct);
    }
}
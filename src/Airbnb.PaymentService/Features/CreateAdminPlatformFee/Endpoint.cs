using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Features.CreateAdminPlatformFee;

public class Request
{
    public decimal FeePercentage { get; set; }
    public string? Description { get; set; }
}

public class Endpoint(IMediator mediator)
    : FastEndpoints.Endpoint<Request, ApiResponse<PlatformFeeCreateResponse>>
{
    public override void Configure()
    {
        Post("/api/platform-fee/admin");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Admin: create new platform fee configuration";
            s.Description = "Creates a new platform fee rate. The previous active rate is deactivated. " +
                            "Fee percentage must be between 0% and 50%. " +
                            "Requires senior Admin with Finance permission.";
            s.Responses[200] = "New platform fee configuration created.";
            s.Responses[400] = "Invalid fee percentage.";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var command = new Command(req.FeePercentage, req.Description, Guid.Empty); // TODO: Get admin user ID from claims
        var result = await mediator.Send(command, ct);
        await Send.ResponseAsync(ApiResponse<PlatformFeeCreateResponse>.SuccessResult(result), cancellation: ct);
    }
}

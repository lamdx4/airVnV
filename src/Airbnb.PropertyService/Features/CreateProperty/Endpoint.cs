using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.CreateProperty;

/// <summary>
/// Endpoint chỉ làm 1 việc: bridge HTTP → Mediator → Response.
/// Không có business logic. Không biết DB, Domain tồn tại.
/// </summary>
public class Endpoint(IMediator mediator)
    : FastEndpoints.Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Post("/api/properties");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Create a new property listing (Draft)";
            s.Description = "Possible Error Codes: \n" +
                            "- **PROPERTY_TITLE_REQUIRED**: Title is mandatory.\n" +
                            "- **PROPERTY_SLUG_REQUIRED**: Slug is mandatory.\n" +
                            "- **PROPERTY_HOST_REQUIRED**: Host identifier is missing.";
            s.Responses[201] = "Property created successfully in Draft mode.";
            s.Responses[400] = "Validation or business rule failure.";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var userIdStr = HttpContext.Request.Headers["X-User-Id"].ToString();
        if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
        {
            await Send.ResponseAsync(ApiResponse<Response>.FailureResult("UNAUTHORIZED", "User identification missing."), 401, ct);
            return;
        }

        var requestWithUser = req with { HostId = userId };
        var result = await mediator.Send(requestWithUser, ct);
        await Send.ResponseAsync(ApiResponse<Response>.SuccessResult(result), cancellation: ct);
    }
}

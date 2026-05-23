using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;
using System.Text.Json;

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
        AllowFileUploads(); // Quan trọng: Cho phép nhận multipart/form-data
        Summary(s =>
        {
            s.Summary = "Create a new property listing with images (Atomic)";
            s.Description = "Possible Error Codes: \n" +
                            "- **PROPERTY_TITLE_REQUIRED**: Title is mandatory.\n" +
                            "- **PROPERTY_SLUG_REQUIRED**: Slug is mandatory.\n" +
                            "- **PROPERTY_HOST_REQUIRED**: Host identifier is missing.";
            s.Responses[201] = "Property created successfully with images.";
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

        CreatePropertyDto? payloadDto;
        try
        {
            // Deserialize using the Source Generator Context
            payloadDto = JsonSerializer.Deserialize(req.Payload, Infrastructure.PropertyJsonContext.Default.CreatePropertyDto);
            if (payloadDto == null) throw new Exception("Payload is null after deserialization");
        }
        catch (Exception ex)
        {
            await Send.ResponseAsync(ApiResponse<Response>.FailureResult("INVALID_PAYLOAD", "Cannot parse Payload JSON string: " + ex.Message), 400, ct);
            return;
        }

        var command = new CreatePropertyCommand(payloadDto, req.Images, userId);
        var result = await mediator.Send(command, ct);
        
        await Send.ResponseAsync(ApiResponse<Response>.SuccessResult(result), cancellation: ct);
    }
}

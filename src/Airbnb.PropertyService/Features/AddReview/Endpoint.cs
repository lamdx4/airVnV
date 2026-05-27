using Airbnb.ServiceDefaults.Infrastructure;
using FastEndpoints;
using Mediator;

namespace Airbnb.PropertyService.Features.AddReview;

public class Endpoint(IMediator mediator) : Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Post("/api/properties/{PropertyId}/reviews");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var userIdStr = HttpContext.Request.Headers["X-User-Id"].ToString();
        if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var guestId))
        {
            await Send.ResponseAsync(ApiResponse<Response>.FailureResult("UNAUTHORIZED", "User identification missing. Please log in."), 401, ct);
            return;
        }

        // Bơm GuestId từ Header vào Request để tránh giả mạo
        var secureReq = req with { GuestId = guestId };

        var result = await mediator.Send(secureReq, ct);
        await Send.ResponseAsync(ApiResponse<Response>.SuccessResult(result), cancellation: ct);
    }
}

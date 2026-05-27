using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.UpdateReview;

public class Endpoint(IMediator mediator) : Endpoint<UpdateReviewRequest, ApiResponse<UpdateReviewResponse>>
{
    public override void Configure()
    {
        Put("/api/properties/{PropertyId}/reviews/{ReviewId}");
    }

    public override async Task HandleAsync(UpdateReviewRequest req, CancellationToken ct)
    {
        // Extract X-User-Id from headers for authorization
        var userIdHeader = HttpContext.Request.Headers["X-User-Id"].FirstOrDefault();
        if (string.IsNullOrEmpty(userIdHeader) || !Guid.TryParse(userIdHeader, out var guestId))
        {
            await Send.ResponseAsync(ApiResponse<UpdateReviewResponse>.FailureResult("AUTH_ERROR", "Unauthorized or invalid user identity."), 401, ct);
            return;
        }

        req.GuestId = guestId;

        var result = await mediator.Send(req, ct);
        await Send.ResponseAsync(ApiResponse<UpdateReviewResponse>.SuccessResult(result), cancellation: ct);
    }
}

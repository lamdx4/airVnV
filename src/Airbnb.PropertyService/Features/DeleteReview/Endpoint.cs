using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.DeleteReview;

public class Endpoint(IMediator mediator) : Endpoint<DeleteReviewRequest, ApiResponse<DeleteReviewResponse>>
{
    public override void Configure()
    {
        Delete("/api/properties/{PropertyId}/reviews/{ReviewId}");
    }

    public override async Task HandleAsync(DeleteReviewRequest req, CancellationToken ct)
    {
        // Extract X-User-Id from headers for authorization
        var userIdHeader = HttpContext.Request.Headers["X-User-Id"].FirstOrDefault();
        if (string.IsNullOrEmpty(userIdHeader) || !Guid.TryParse(userIdHeader, out var guestId))
        {
            await Send.ResponseAsync(ApiResponse<DeleteReviewResponse>.FailureResult("AUTH_ERROR", "Unauthorized or invalid user identity."), 401, ct);
            return;
        }

        req.GuestId = guestId;

        var result = await mediator.Send(req, ct);
        await Send.ResponseAsync(ApiResponse<DeleteReviewResponse>.SuccessResult(result), cancellation: ct);
    }
}

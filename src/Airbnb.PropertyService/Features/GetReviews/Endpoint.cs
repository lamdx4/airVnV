using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.GetReviews;

public class Endpoint(IMediator mediator) : Endpoint<GetReviewsRequest, ApiResponse<GetReviewsResponse>>
{
    public override void Configure()
    {
        Get("/api/properties/{PropertyId}/reviews");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetReviewsRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await Send.ResponseAsync(ApiResponse<GetReviewsResponse>.SuccessResult(result), cancellation: ct);
    }
}

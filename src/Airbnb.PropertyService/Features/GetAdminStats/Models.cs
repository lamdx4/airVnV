using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.GetAdminStats;

public record Request : Mediator.IQuery<ApiResponse<PropertyStatsResponse>>;

public record PropertyStatsResponse(
    int TotalProperties,
    int PendingReview,
    int Published,
    int Suspended,
    int TotalReviews
);

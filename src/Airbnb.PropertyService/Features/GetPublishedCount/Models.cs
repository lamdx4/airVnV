using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.GetPublishedCount;

public record Request : Mediator.IQuery<ApiResponse<PublishedCountResponse>>;

public record PublishedCountResponse(int Published);

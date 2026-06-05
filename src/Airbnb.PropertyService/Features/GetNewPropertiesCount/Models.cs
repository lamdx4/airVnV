using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.GetNewPropertiesCount;

public record Request(
    [property: BindFrom("from")] DateOnly From,
    [property: BindFrom("to")] DateOnly To
) : Mediator.IQuery<ApiResponse<NewPropertiesCountResponse>>;

public record NewPropertiesCountResponse(int NewProperties);

using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.GetPropertiesByIds;

public record Request(
    [property: BindFrom("ids")] Guid[] Ids
) : Mediator.IQuery<ApiResponse<List<PropertyBasicInfo>>>;

public record PropertyBasicInfo(
    Guid Id, 
    string Title,
    decimal Price,
    string Currency,
    decimal Rating,
    string DisplayAddress,
    double Latitude,
    double Longitude,
    List<string> Images
);

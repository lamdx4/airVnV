using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.SearchService.Features.SearchProperties;

public record Request(
    double Latitude,
    double Longitude,
    double RadiusKm = 10,
    int Page = 1,
    int PageSize = 20
) : IQuery<PagedResponse<Domain.PropertyDoc>>;

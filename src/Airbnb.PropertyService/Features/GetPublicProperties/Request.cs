using Airbnb.ServiceDefaults.Infrastructure;
using Mediator;
using Airbnb.PropertyService.Features.GetAdminProperties;

namespace Airbnb.PropertyService.Features.GetPublicProperties;

public record Request(
    int Page = 1,
    int PageSize = 20,
    int? PropertyType = null
) : IQuery<PagedResponse<Response>>;

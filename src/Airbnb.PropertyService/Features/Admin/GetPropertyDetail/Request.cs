using Mediator;

namespace Airbnb.PropertyService.Features.Admin.GetPropertyDetail;

public record Request(Guid PropertyId) : IQuery<AdminPropertyDetailDto>;

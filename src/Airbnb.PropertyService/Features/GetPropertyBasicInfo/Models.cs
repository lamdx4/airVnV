using Mediator;

namespace Airbnb.PropertyService.Features.GetPropertyBasicInfo;

public record Request(Guid PropertyId) : IQuery<Response>;

public record Response(Guid PropertyId, string Title, Guid HostId);

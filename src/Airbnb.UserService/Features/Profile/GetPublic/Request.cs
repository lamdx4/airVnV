using Mediator;

namespace Airbnb.UserService.Features.Profile.GetPublic;

public record Request(Guid UserId) : IQuery<Response>;

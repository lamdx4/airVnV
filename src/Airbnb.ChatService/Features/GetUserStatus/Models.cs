using Mediator;

namespace Airbnb.ChatService.Features.GetUserStatus;

public record Request(Guid UserId) : IQuery<Response>;

public record Response(bool IsOnline);

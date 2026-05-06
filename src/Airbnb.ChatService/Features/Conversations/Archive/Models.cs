using Mediator;

namespace Airbnb.ChatService.Features.Conversations.Archive;

public record Request(Guid ConversationId, Guid UserId) : ICommand<Response>;

public record Response(bool Success);

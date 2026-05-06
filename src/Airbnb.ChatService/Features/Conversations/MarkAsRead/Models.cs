using Mediator;

namespace Airbnb.ChatService.Features.Conversations.MarkAsRead;

public record Request(Guid ConversationId, Guid UserId, Guid LastReadMessageId) : ICommand<Response>;

public record Response(bool Success);

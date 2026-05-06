using Mediator;

namespace Airbnb.ChatService.Features.Conversations.GetMessages;

public record Request(Guid ConversationId, Guid UserId, DateTimeOffset? Before, int Limit = 50) : IQuery<Response>;

public record Response(List<MessageItem> Items, DateTimeOffset? NextCursor);

public record MessageItem(
    Guid Id,
    Guid? SenderId,
    string Content,
    string MessageType,
    DateTimeOffset CreatedAt
);

using Mediator;

namespace Airbnb.ChatService.Features.Conversations.GetInbox;

public record Request(Guid UserId, DateTimeOffset? Before, int Limit = 20) : IQuery<Response>;

public record Response(List<InboxItem> Items, DateTimeOffset? NextCursor);

public record InboxItem(
    Guid ConversationId,
    string PropertyTitle,
    string OtherParticipantName,
    string? OtherParticipantAvatar,
    int UnreadCount,
    DateTimeOffset LastMessageAt
);

using Mediator;

namespace Airbnb.ChatService.Features.Conversations.GetInbox;

public record Request(Guid UserId, DateTimeOffset? Before, int Limit = 20) : IQuery<Response>;

public record Response(List<InboxItem> Items, DateTimeOffset? NextCursor);

public record InboxItem(
    Guid ConversationId,
    string PropertyTitle,
    string OtherParticipantName,
    string? OtherParticipantAvatar,
    Guid? OtherParticipantId,
    int UnreadCount,
    DateTimeOffset LastMessageAt,
    Guid? OtherLastReadMessageId,
    string? LatestMessageContent,
    Guid? LatestMessageId
);

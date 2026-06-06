using Mediator;

namespace Airbnb.ChatService.Features.Conversations.GetAttachments;

public record Request(Guid ConversationId, Guid UserId, string Type, DateTimeOffset? Before, int Limit = 20) : IQuery<Response>;

public record Response(List<AttachmentItem> Items, DateTimeOffset? NextCursor);

public record AttachmentItem(
    Guid MessageId,
    Guid? SenderId,
    string Content,
    string MessageType,
    DateTimeOffset CreatedAt
);

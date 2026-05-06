using Mediator;

namespace Airbnb.ChatService.Features.Conversations.SendMessage;

public record Request(Guid ConversationId, Guid SenderId, string Content) : ICommand<Response>;

public record Response(Guid MessageId, DateTimeOffset CreatedAt);

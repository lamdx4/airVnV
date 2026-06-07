using Mediator;

namespace Airbnb.ChatService.Features.Conversations.Create;

public record Request(Guid PropertyId, Guid? ReservationId, Guid GuestId) : ICommand<Response>;

public record Response(Guid ConversationId);

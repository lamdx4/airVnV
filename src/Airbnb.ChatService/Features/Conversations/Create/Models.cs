using Mediator;

namespace Airbnb.ChatService.Features.Conversations.Create;

public record Request(Guid PropertyId, Guid? ReservationId, Guid GuestId, string GuestName, string? GuestAvatarUrl) : ICommand<Response>;

public record Response(Guid ConversationId);

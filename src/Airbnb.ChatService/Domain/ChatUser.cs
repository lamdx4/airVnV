namespace Airbnb.ChatService.Domain;

public class ChatUser
{
    public Guid UserId { get; set; }
    public string DisplayName { get; set; } = default!;
    public string? AvatarUrl { get; set; }
    
    // Navigation property for 1-N relationship
    public ICollection<ConversationParticipant> Participations { get; set; } = new List<ConversationParticipant>();
}

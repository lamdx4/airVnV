namespace Airbnb.ChatService.Domain;

public class ConversationParticipant
{
    public Guid ConversationId { get; set; }
    public Guid UserId { get; set; }
    
    public ParticipantRole Role { get; set; }
    

    // Mốc tin nhắn cuối cùng user này đã đọc (UUIDv7 compare)
    public Guid? LastReadMessageId { get; set; }
    
    public bool IsArchived { get; set; } = false;

    // Navigation property
    public Conversation Conversation { get; set; } = default!;
    public ChatUser User { get; set; } = default!;
}

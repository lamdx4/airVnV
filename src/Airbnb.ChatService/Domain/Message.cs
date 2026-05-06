namespace Airbnb.ChatService.Domain;

public class Message
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    
    public Guid ConversationId { get; set; }
    
    // Null = System Message
    public Guid? SenderId { get; set; }
    
    public MessageType MessageType { get; set; } = MessageType.Text;
    
    public string Content { get; set; } = default!;
    
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    // Navigation property
    public Conversation Conversation { get; set; } = default!;
}

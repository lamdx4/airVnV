namespace Airbnb.ChatService.Domain;

public class Conversation
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    
    // Ngữ cảnh căn hộ
    public Guid PropertyId { get; set; }
    
    // Snapshot tên căn hộ (không đồng bộ)
    public string PropertyTitle { get; set; } = default!;
    
    // Ngữ cảnh đặt phòng (nếu có)
    public Guid? ReservationId { get; set; }
    
    // Thời điểm có tin nhắn cuối cùng (dùng để sort inbox)
    public DateTimeOffset LastMessageAt { get; set; } = DateTimeOffset.UtcNow;
    
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    // Navigation property
    public ICollection<ConversationParticipant> Participants { get; set; } = new List<ConversationParticipant>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}

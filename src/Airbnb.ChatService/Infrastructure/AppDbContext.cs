using Airbnb.ChatService.Domain;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.ChatService.Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Conversation> Conversations => Set<Conversation>();
    public DbSet<ConversationParticipant> ConversationParticipants => Set<ConversationParticipant>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<ChatUser> ChatUsers => Set<ChatUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Cấu hình bảng Outbox/Inbox cho MassTransit
        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();

        // ------------------ Conversations ------------------
        modelBuilder.Entity<Conversation>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            // Đảm bảo không tạo 2 conversation cho cùng 1 booking (property + reservation)
            entity.HasIndex(e => new { e.PropertyId, e.ReservationId })
                  .IsUnique()
                  .HasFilter("\"ReservationId\" IS NOT NULL")
                  .HasDatabaseName("idx_conversation_property_reservation_unique");
            
            // Vẫn giữ lại Index thông thường để query cho nhanh
            entity.HasIndex(e => e.PropertyId)
                  .HasDatabaseName("idx_conversation_property");
                  
            entity.HasIndex(e => e.ReservationId)
                  .HasDatabaseName("idx_conversation_reservation");
        });

        // ------------------ ChatUsers ------------------
        modelBuilder.Entity<ChatUser>(entity =>
        {
            entity.HasKey(e => e.UserId);
        });

        // ------------------ ConversationParticipants ------------------
        modelBuilder.Entity<ConversationParticipant>(entity =>
        {
            // Composite Primary Key
            entity.HasKey(e => new { e.ConversationId, e.UserId });
            
            // Index cho việc load Inbox nhanh
            // Entity Framework Core hiện tại không hỗ trợ trực tiếp INCLUDE qua Fluent API một cách phổ biến
            // Nên ta có thể tạo index bình thường, hoặc tạo Include qua migration raw sql.
            entity.HasIndex(e => e.UserId)
                  .HasDatabaseName("idx_participants_user_id");

            entity.Property(e => e.Role).HasConversion<string>();

            entity.HasOne(e => e.User)
                  .WithMany(u => u.Participations)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ------------------ Messages ------------------
        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            // Index để phân trang và tính unread count nhanh bằng cách so sánh Id (UUIDv7)
            entity.HasIndex(e => new { e.ConversationId, e.Id })
                  .HasDatabaseName("idx_messages_conversation_id");
                  
            entity.Property(e => e.MessageType).HasConversion<string>();
        });
    }
}

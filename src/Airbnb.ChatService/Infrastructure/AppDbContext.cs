using Airbnb.ChatService.Domain;
using Airbnb.ChatService.Domain.Tickets;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.ChatService.Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Existing
    public DbSet<Conversation> Conversations => Set<Conversation>();
    public DbSet<ConversationParticipant> ConversationParticipants => Set<ConversationParticipant>();
    public DbSet<Message> Messages => Set<Message>();

    // New - Support Tickets (Epic D)
    public DbSet<SupportTicket> SupportTickets => Set<SupportTicket>();
    public DbSet<TicketComment> TicketComments => Set<TicketComment>();
    public DbSet<TicketAttachment> TicketAttachments => Set<TicketAttachment>();
    public DbSet<RefundRequest> RefundRequests => Set<RefundRequest>();

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
            
            // Partial Unique Indexes chống duplicate
            // Khi không có reservation
            entity.HasIndex(e => e.PropertyId)
                  .IsUnique()
                  .HasFilter("\"ReservationId\" IS NULL")
                  .HasDatabaseName("uq_conversation_property_no_res");
                  
            // Khi có reservation
            entity.HasIndex(e => new { e.PropertyId, e.ReservationId })
                  .IsUnique()
                  .HasFilter("\"ReservationId\" IS NOT NULL")
                  .HasDatabaseName("uq_conversation_property_res");
        });

        // ------------------ ConversationParticipants ------------------
        modelBuilder.Entity<ConversationParticipant>(entity =>
        {
            // Composite Primary Key
            entity.HasKey(e => new { e.ConversationId, e.UserId });
            
            // Index cho việc load Inbox nhanh
            entity.HasIndex(e => e.UserId)
                  .HasDatabaseName("idx_participants_user_id");

            entity.Property(e => e.Role).HasConversion<string>();
        });

        // ------------------ Messages ------------------
        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            // Index để phân trang
            entity.HasIndex(e => new { e.ConversationId, e.CreatedAt })
                  .IsDescending(false, true)
                  .HasDatabaseName("idx_messages_conversation_created");
                  
            entity.Property(e => e.MessageType).HasConversion<string>();
        });

        // ------------------ Support Tickets (Epic D) ------------------
        modelBuilder.Entity<SupportTicket>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(20);
            entity.Property(e => e.Priority).HasConversion<string>().HasMaxLength(20);
            entity.Property(e => e.Category).HasConversion<string>().HasMaxLength(20);
            
            entity.HasIndex(e => e.Status).HasDatabaseName("idx_tickets_status");
            entity.HasIndex(e => e.Priority).HasDatabaseName("idx_tickets_priority");
            entity.HasIndex(e => e.CreatedAt).HasDatabaseName("idx_tickets_created");
            entity.HasIndex(e => e.ReporterId).HasDatabaseName("idx_tickets_reporter");
            entity.HasIndex(e => e.AssignedToId).HasDatabaseName("idx_tickets_assigned");
        });

        modelBuilder.Entity<TicketComment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Ticket).WithMany(t => t.Comments).HasForeignKey(e => e.TicketId).OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => e.TicketId).HasDatabaseName("idx_ticket_comments_ticket");
        });

        modelBuilder.Entity<TicketAttachment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Ticket).WithMany(t => t.Attachments).HasForeignKey(e => e.TicketId).OnDelete(DeleteBehavior.Cascade);
        });

        // ------------------ Refund Requests (Epic D) ------------------
        modelBuilder.Entity<RefundRequest>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(20);
            entity.Property(e => e.Type).HasConversion<string>().HasMaxLength(20);
            entity.Property(e => e.Currency).HasMaxLength(3);
            entity.Property(e => e.RequestedAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
            
            entity.HasIndex(e => e.BookingId).HasDatabaseName("idx_refunds_booking");
            entity.HasIndex(e => e.GuestId).HasDatabaseName("idx_refunds_guest");
            entity.HasIndex(e => e.Status).HasDatabaseName("idx_refunds_status");
            entity.HasIndex(e => e.CreatedAt).HasDatabaseName("idx_refunds_created");
        });
    }
}

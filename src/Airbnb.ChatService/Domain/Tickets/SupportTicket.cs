namespace Airbnb.ChatService.Domain.Tickets;

public enum TicketStatus { Open, InProgress, Resolved, Closed, Escalated }
public enum TicketPriority { Low, Medium, High, Urgent }
public enum TicketCategory { Booking, Payment, Property, Host, Guest, Other }

public class SupportTicket
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    
    // Ticket Info
    public string Subject { get; set; } = default!;
    public string Description { get; set; } = default!;
    public TicketCategory Category { get; set; }
    public TicketPriority Priority { get; set; } = TicketPriority.Medium;
    public TicketStatus Status { get; set; } = TicketStatus.Open;
    
    // Context
    public Guid? BookingId { get; set; }
    public Guid? PropertyId { get; set; }
    public Guid? ConversationId { get; set; }
    
    // Reporter
    public Guid ReporterId { get; set; }
    public string ReporterEmail { get; set; } = default!;
    public string ReporterName { get; set; } = default!;
    public bool IsReporterHost { get; set; }
    
    // Assignment
    public Guid? AssignedToId { get; set; }       // Admin user ID
    public string? AssignedToName { get; set; }
    public DateTimeOffset? AssignedAt { get; set; }
    
    // Resolution
    public string? Resolution { get; set; }
    public Guid? ResolvedById { get; set; }
    public DateTimeOffset? ResolvedAt { get; set; }
    
    // Timestamps
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    
    // Navigation
    public ICollection<TicketComment> Comments { get; set; } = new List<TicketComment>();
    public ICollection<TicketAttachment> Attachments { get; set; } = new List<TicketAttachment>();
}

public class TicketComment
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid TicketId { get; set; }
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = default!;
    public string Content { get; set; } = default!;
    public bool IsInternal { get; set; } // Internal notes (not visible to reporter)
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    
    // Navigation
    public SupportTicket Ticket { get; set; } = default!;
}

public class TicketAttachment
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid TicketId { get; set; }
    public string FileName { get; set; } = default!;
    public string FileUrl { get; set; } = default!;
    public string FileType { get; set; } = default!;
    public long FileSize { get; set; }
    public DateTimeOffset UploadedAt { get; set; } = DateTimeOffset.UtcNow;
    
    // Navigation
    public SupportTicket Ticket { get; set; } = default!;
}

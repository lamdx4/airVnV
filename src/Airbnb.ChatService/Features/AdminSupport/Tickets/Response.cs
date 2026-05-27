namespace Airbnb.ChatService.Features.AdminSupport.Tickets;

public class TicketResponse
{
    public Guid Id { get; set; }
    public string Subject { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string Category { get; set; } = default!;
    public string Priority { get; set; } = default!;
    public string Status { get; set; } = default!;
    public Guid? BookingId { get; set; }
    public Guid? PropertyId { get; set; }
    public Guid ReporterId { get; set; }
    public string ReporterEmail { get; set; } = default!;
    public string ReporterName { get; set; } = default!;
    public bool IsReporterHost { get; set; }
    public Guid? AssignedToId { get; set; }
    public string? AssignedToName { get; set; }
    public DateTimeOffset? AssignedAt { get; set; }
    public string? Resolution { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public List<TicketCommentResponse> Comments { get; set; } = new();
    public int CommentCount { get; set; }
    public int AttachmentCount { get; set; }
}

public class TicketCommentResponse
{
    public Guid Id { get; set; }
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = default!;
    public string Content { get; set; } = default!;
    public bool IsInternal { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class TicketListResponse
{
    public List<TicketSummaryResponse> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public TicketStats Stats { get; set; } = new();
}

public class TicketSummaryResponse
{
    public Guid Id { get; set; }
    public string Subject { get; set; } = default!;
    public string Category { get; set; } = default!;
    public string Priority { get; set; } = default!;
    public string Status { get; set; } = default!;
    public string ReporterName { get; set; } = default!;
    public string? AssignedToName { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public int CommentCount { get; set; }
}

public class TicketStats
{
    public int TotalOpen { get; set; }
    public int TotalInProgress { get; set; }
    public int TotalResolved { get; set; }
    public int TotalEscalated { get; set; }
    public int HighPriorityCount { get; set; }
    public int UrgentCount { get; set; }
}
